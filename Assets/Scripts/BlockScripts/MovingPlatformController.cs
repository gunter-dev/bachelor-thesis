using System.Collections.Generic;
using UnityEngine;

namespace BlockScripts
{
    public class MovingPlatformController : MonoBehaviour
    {
        public List<PlatformInfo> path;

        private int _nextIndex;

        private void Start()
        {
            InstantiateOtherPlatformParts();
        }

        private void Update()
        {
            MoveBlock();
        }

        private void InstantiateOtherPlatformParts()
        {
            for (short i = 1; i < path[_nextIndex].size; i++)
            {
                var position = transform.position;
                GameObject platform = Resources.Load<GameObject>("Grounds/Moving Platform");
                platform = Instantiate(platform, new Vector3(position.x + i, position.y, 1), Quaternion.identity);

                List<PlatformInfo> newPath = new List<PlatformInfo>();
                foreach (var item in path)
                    newPath.Add(new PlatformInfo(item.x + i, item.y, item.colorCode, 0));

                platform.GetComponent<MovingPlatformController>().path = newPath;
            }
        }

        private void MoveBlock()
        {
            Vector2 nextPoint = new Vector2(path[_nextIndex].x, path[_nextIndex].y);

            transform.position = Vector2.MoveTowards(transform.position, nextPoint, Constants.PlatformSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, nextPoint) < Constants.DistanceTolerance)
                IncrementIndex();
        }

        private void IncrementIndex()
        {
            _nextIndex = _nextIndex == path.Count - 1 ? 0 : _nextIndex + 1;
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            col.transform.SetParent(transform);
        }

        private void OnCollisionExit2D(Collision2D col)
        {
            col.transform.SetParent(null);
        }
    }
}
