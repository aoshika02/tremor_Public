using System.Collections.Generic;
using UnityEngine;

public class DecisionGround : SingletonMonoBehaviour<DecisionGround>
{
    RaycastHit hitInfo;

    [SerializeField] Transform _playerTransform;
    public class GroundTypeData
    {
        public int Type { private set; get; }
        public float Ratio{ private set; get; }

        public void AddType(int type) 
        {
            Type = type;
        }
        public void AddRatio(float ratio) 
        {
            Ratio = ratio;
        }
       }
    public List<GroundTypeData> GetGroundType()
    {
        
        List<GroundTypeData> groundTypeDataList = new List<GroundTypeData>();

        var pos = _playerTransform.position;
        pos.y += 1;
        Debug.DrawRay(pos, Vector3.down,Color.red,0.1f);
        if (Physics.Raycast(pos, Vector3.down, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Default")))
        {
            string hitColliderTag = hitInfo.collider.tag;

            if (hitColliderTag == "Terrain")
            {
                // テレインデータ
                TerrainData terrainData = hitInfo.collider.gameObject.GetComponent<Terrain>().terrainData;

                // アルファマップ 
                float[,,] alphaMaps = terrainData.GetAlphamaps(Mathf.FloorToInt(hitInfo.textureCoord.x * terrainData.alphamapWidth), Mathf.FloorToInt(hitInfo.textureCoord.y * terrainData.alphamapHeight), 1, 1);

                int layerCount = terrainData.alphamapLayers; // テレインレイヤーの数

                float[] slatmap = new float[layerCount];
                for (int i = 0; i < layerCount; i++)
                {
                    slatmap[i] = alphaMaps[0, 0, i];
                    //アルファマップに値が入っているレイヤーだったらリストに追加する
                    if (slatmap[i] != 0)
                    {
                        var groundTypeData = new GroundTypeData();
                        groundTypeData.AddType(i);
                        groundTypeData.AddRatio((float)slatmap[i]);
                        groundTypeDataList.Add(groundTypeData);
                    }
                }
                return groundTypeDataList;
            }
            else
            {
                return groundTypeDataList;
            }
        }
        else
        {
            return groundTypeDataList;
        }
    }
}
