
namespace DungeonDefence
{
	using UnityEngine;

	public static class GameConstants
	{
		public static int _ROWS = 45;
		public static int _COLUMNS = 45;
		public static int _COLLISION_GRID_PRECISION = 20; // OR SCALE
		public static float UNIT_VISION_RANGE = 10f; // Only distance, no ray cast
		public static float PROJECTILE_SPEED = 80f;
		public static float PLAYER_ATTACK_PUSHBACK = 0.2f;

	}
}
