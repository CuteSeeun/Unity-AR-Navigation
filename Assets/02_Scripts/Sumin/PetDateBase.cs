using UnityEngine;

namespace ARNavi.Sumin
{
    /// <summary>
    /// ARPet �����ϴ� �����ͺ��̽�
    /// </summary>
    [CreateAssetMenu]
    public class PetDataBase : ScriptableObject
    {
        //�����ͺ��̽��� ����� ��� �� ������ ����
        public Pet[] pets;

        /// <summary>
        /// ���� �� ���� ��ȯ
        /// </summary>
        public int PetCount
        {
            get
            {
                return pets.Length;
            }
        }
        /// <summary>
        /// ���� ���� ��ȯ
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Pet GetPet(int index)
        {
            return (pets[index]);
        }
    }
}
