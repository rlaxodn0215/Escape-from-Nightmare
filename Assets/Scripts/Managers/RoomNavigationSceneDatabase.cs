using System;
using System.Collections.Generic;
using UnityEngine;

namespace EscapeFromNightmare
{
	public class RoomNavigationSceneDatabase : MonoBehaviour
	{
		[SerializeField] private RoomExit[] exits = Array.Empty<RoomExit>();
		[SerializeField] private RoomHidePoint[] hidePoints = Array.Empty<RoomHidePoint>();

		public IReadOnlyList<RoomExit> Exits => exits;
		public IReadOnlyList<RoomHidePoint> HidePoints => hidePoints;
	}
}
