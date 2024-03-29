namespace DungeonDefence
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class BattleUnit : MonoBehaviour
    {

        public Data.UnitID id = Data.UnitID.barbarian;
        private Vector3 lastPosition = Vector3.zero;
        private int i = -1; public int index { get { return i; }   set { i = value; } }
        private long _id = 0; public long databaseID { get { return _id; } }
         public MeshRenderer baseArea = null;
        [HideInInspector] public UI_Bar healthBar = null;
        [HideInInspector] public Data.Unit data = null;

        public void Initialize(int index, long id, Data.Unit unit) 
        {
            data = unit;
            _id = id;
            i = index;
            lastPosition = transform.position;
        }

        private void Update()
        {
            if(transform.position != lastPosition)
            {
                Vector3 direction = transform.position - lastPosition;
                lastPosition = transform.position;
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        public void PlacedOnGrid(int x, int y)
		{
	
		//	Vector3 position = UI_Main.instance._grid.GetCenterPosition(x,y, 1, 1);
		//	transform.position = position;
		}
    }
}