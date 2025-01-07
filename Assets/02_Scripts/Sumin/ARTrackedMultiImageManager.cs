using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARNavi.Sumin
{
    public class ARTrackedMultiImageManager : MonoBehaviour
    {
        //이미지 인식시 출력되는 프리팹
        [SerializeField]
        private GameObject[] tracked;
        //이미지 인식시 출력되는 오브젝트
        private Dictionary<string, GameObject> spawend = new Dictionary<string, GameObject>();
        private ARTrackedImageManager imageManager;

        private void Awake()
        {
            imageManager = GetComponent<ARTrackedImageManager>();

            foreach (GameObject prefab in tracked)
            {
                GameObject clone = Instantiate(prefab);
                clone.name = prefab.name;
                clone.SetActive(false);
                spawend.Add(clone.name, clone);
            }
        }

        private void OnEnable()
        {
            imageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }
        private void OnDisable()
        {
            imageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }

        /// <summary>
        /// 이미지 트래킹
        /// </summary>
        /// <param name="eventArgs"></param>
        private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
        {
            foreach (var trackedimg in eventArgs.added)
            {
                UpdateImage(trackedimg);
            }
            foreach (var trackedimg in eventArgs.updated)
            {
                UpdateImage(trackedimg);
            }
            foreach (var trackedimg in eventArgs.removed)
            {
                spawend[trackedimg.name].SetActive(false);
            }
        }

        /// <summary>
        /// 이미지 업데이트
        /// </summary>
        /// <param name="trackedimg"></param>
        private void UpdateImage(ARTrackedImage trackedimg)
        {
            string name = trackedimg.referenceImage.name;
            GameObject trackedobj = spawend[name];

            if (trackedimg.trackingState == TrackingState.Tracking)
            {
                trackedobj.transform.position = trackedimg.transform.position;
                trackedobj.transform.rotation = trackedimg.transform.rotation;
                trackedobj.SetActive(true);
            }
            else
            {
                trackedobj.SetActive(false);
            }
        }
    }
}
