using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/AnimatedTile")]
public class AnimatedTile : TileBase
{
    public Sprite[] frames;
    public float frameDuration = 0.25f;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        if (frames != null && frames.Length > 0)
        {
            // Pick a frame based on time
            int index = (int)(Time.time / frameDuration) % frames.Length;
            tileData.sprite = frames[index];
        }
    }

    public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
    {
        if (frames == null || frames.Length == 0)
            return false;

        tileAnimationData.animatedSprites = frames;
        tileAnimationData.animationSpeed = 1f / frameDuration;
        tileAnimationData.animationStartTime = 0f;
        return true;
    }
}
