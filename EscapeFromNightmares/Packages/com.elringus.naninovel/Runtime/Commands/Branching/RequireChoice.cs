using System;
using Naninovel.Metadata;
using UnityEngine;

namespace Naninovel.Commands
{
    [Doc(
        @"
Adds a required [choice](/guide/choices) option, which halts further scenario playback until the player makes a selection.  
Subsequent choice commands are merged, allowing multiple options to be presented at once.
Use [@addChoice] instead of this command to simply add a choice, without requiring a selection before proceeding with the playback.",
        @"
When nesting commands under the choice, `goto`, `gosub` and `set` parameters are ignored.

Using non-deterministic expressions in the if parameter is not supported, because the command must determine
in advance which choice is the last in the chain in order to stop playback automatically.
If you need something like `@choice ... if:random(0,10)>5`, use the [@addChoice] command instead.

Labels between the subsequent choice commands are ignored.",
        @"
; Print the text, then immediately show choices and halt the playback
; until one of the choices is selected.
Continue executing this script or ...?[>]
@choice ""Continue""
@choice ""Load another script from start"" goto:Another
@choice ""Load another script from \""Label\"" label"" goto:Another#Label
@choice ""Goto to \""Sub\"" subroutine in another script"" gosub:Another#Sub",
        @"
; Set custom variables based on choices.
@choice ""I'm humble, one is enough..."" set:score++
@choice ""Two, please."" set:score=score+2
@choice ""I'll take the entire stock!"" set:karma--;score=999",
        @"
; Play a sound effect and arrange characters when the choice is selected.
@choice ""Arrange""
    @sfx Click
    @arrange k.10,y.55",
        @"
; Print a text line corresponding to the selected choice.
@choice ""Ask about color""
    What's your favorite color?
@choice ""Ask about age""
    How old are you?
@choice ""Keep silent""
    ...",
        @"
; Make the choice disabled/locked when 'score' variable is below 10.
@choice ""Extra option"" lock:score<10",
        @"
; Only show the choice when 'score' variable is 10 or more.
@choice ""Secret option"" if:score>=10"
    )]
    [Serializable, Alias("choice"), BranchingGroup, Icon("Circle"), Branch(BranchTraits.Interactive | BranchTraits.Nest | BranchTraits.Endpoint)]
    public class RequireChoice : AddChoice
    {
        public override async Awaitable Execute (ExecutionContext ctx)
        {
            await base.Execute(ctx);
            if (ShouldStop(ctx.Track)) ctx.Track.Stop();
        }

        protected virtual bool ShouldStop (IScriptTrack track)
        {
            // walk the choice chain...
            var idx = track.Playlist.MoveAt(track.PlayedIndex);
            while (track.Playlist.GetCommandByIndex(idx) is { } cmd)
                // next is some other command – stop the player
                if (cmd is not AddChoice nextChoice) return true;
                // next is @choice and it'll execute — delegate the decision to them
                else if (nextChoice.ShouldExecute) return false;
                // next is @choice that won't execute – continue walking
                else idx = track.Playlist.MoveAt(idx);
            // next is nothing (script finished) - stop the player
            return true;
        }
    }
}
