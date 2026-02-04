using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generatedPlatformLogic : MonoBehaviour
{
    public int length = 30;
    public Sprite[] sprites;

    public void createObject(int n, int i, GameObject root, Vector2 spriteSize, Sprite sprite, int y){
        GameObject tile = new GameObject($"Tile_{n}");
        tile.transform.SetParent(root.transform, worldPositionStays: false); // changed from this.transform to root.transform
        tile.transform.localPosition = new Vector3(n * spriteSize.x+i, y, 0f); // use proper spacing based on sprite size

        SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;

        BoxCollider2D tileCollider = tile.AddComponent<BoxCollider2D>();
        tileCollider.size = spriteSize;
        tileCollider.offset = Vector2.zero;

        tile.tag = "Ground";
        tile.layer = LayerMask.NameToLayer("Ground");
    }

    public GameObject CreateTiledObject(int length, Sprite[] sprites, string parentName = "Platforms")
    {

        

        // 1) Parent container
        GameObject root = new GameObject(parentName);
        root.transform.position = transform.position;

        // 2) Determine sprite size in world units
        

        
        // 4) Create child tiles
        int n = 0;
        while (n < length)
        {

            Sprite sprite = sprites[Random.Range(0, sprites.Length)];
            Vector2 spriteSize = sprite.bounds.size;
            
            int l = Random.Range(2, 5);
            int y = Random.Range(-1, 1);

            if (l+n>length) {
                break;
            }

            for (int i = 0;i<l;i++) {
                createObject(n, i, root, spriteSize, sprite, y);
            }


            n += Random.Range(3, 7);
        }

        return root;
    }

    void generate()
    {
        CreateTiledObject(length, sprites);
    }

    void Start()
    {
        generate();
    }
}
