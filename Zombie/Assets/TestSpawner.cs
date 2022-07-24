using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawner : MonoBehaviour
{
    private Camera _mainCam;
    public GameObject itemPrefab;

    private void Start()
    {
        _mainCam = Camera.main;
    }

    private void Update()
    {
        // 마우스 primary버튼클릭
        if (Input.GetMouseButtonDown(0))
        {
            // 그 지점을 얻어내서
            Ray mouseRay = _mainCam.ScreenPointToRay(Input.mousePosition);

            LayerMask targetLayer = LayerMask.NameToLayer("Ground");
            //int layerMask = (1 << targetLayer.value);

            RaycastHit hit;
            if (Physics.Raycast(mouseRay.origin, mouseRay.direction, out hit, 100f, targetLayer))
            {
                Vector3 spawnPosition = hit.point;
                spawnPosition.y += 0.4f;
                GameObject item = Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
                Destroy(item, 4f);
            }
        }
    }
}
