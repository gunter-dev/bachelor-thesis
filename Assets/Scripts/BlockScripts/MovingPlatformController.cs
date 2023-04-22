using System.Collections.Generic;
using UnityEngine;

namespace BlockScripts
{
    public class MovingPlatformController : MonoBehaviour
    {
        public List<Vector2> path;

        [SerializeField] private bool manual;
        [SerializeField] private Transform[] pathPoints;

        public int size;
        
        private int _nextIndex;

        private void Start()
        {
            if (manual) InitializePath();
            InstantiateOtherPlatformParts();
        }

        private void Update()
        {
            MoveBlock();
        }

        private void InitializePath()
        {
            foreach (var point in pathPoints)
                path.Add(point.position);
        }

        private void InstantiateOtherPlatformParts()
        {
            for (short i = 1; i < size; i++)
            {
                var position = transform.position;
                GameObject platform = Resources.Load<GameObject>("Grounds/Moving Platform");
                platform = Instantiate(platform, new Vector3(position.x + i, position.y, 1), Quaternion.identity);

                List<Vector2> newPath = new List<Vector2>();
                foreach (var item in path)
                    newPath.Add(new Vector2(item.x + i, item.y));

                platform.GetComponent<MovingPlatformController>().path = newPath;
            }
        }

        private void MoveBlock()
        {
            Vector2 nextPoint = path[_nextIndex];

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
