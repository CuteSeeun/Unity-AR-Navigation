using ARNavi.M;
using UnityEngine;

namespace ARNavi.C
{
/// <summary>
/// ARPet 관리하는 데이터베이스
/// </summary>
[CreateAssetMenu]
public class PetDatabase : ScriptableObject
{
        //데이터베이스에 저장된 모든 펫 정보를 포함
        public Pet[] pets;

        /// <summary>
        /// 팻의 총 수를 반환
        /// </summary>
        public int PetCount
        {
            get
            {
                return pets.Length;
            }
        }
        /// <summary>
        /// 팻의 정보 반환
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Pet GetPet(int index)
        {
            return (pets[index]);
        }
    }
}
