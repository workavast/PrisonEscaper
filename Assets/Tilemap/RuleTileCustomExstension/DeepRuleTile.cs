using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "CustomTileMap/DeepRuleTile")]
public class DeepRuleTile : RuleTile<DeepRuleTile.Neighbor> {
    public class Neighbor : RuleTile.TilingRule.Neighbor {
        public const int Empty = 3;
        public const int OtherTile = 4;
    }

    public override bool RuleMatch(int neighbor, TileBase tile) {
        if (tile is RuleOverrideTile ot)
            tile = ot.m_InstanceTile;
        
        switch (neighbor) {
            case TilingRuleOutput.Neighbor.This: return CheckThis(tile);
            case TilingRuleOutput.Neighbor.NotThis: return CheckNotThis(tile);
            case Neighbor.Empty: return CheckEmpty(tile);
            case Neighbor.OtherTile: return CheckOtherTile(tile);
        }
        return true;
    }

    bool CheckThis(TileBase tile)
    {
        return tile == this;
    }
    
    bool CheckNotThis(TileBase tile)
    {
        return tile != this;
    }
    
    bool CheckEmpty(TileBase tile)
    {
        return tile == null;
    }
    
    bool CheckOtherTile(TileBase tile)
    {
        return tile != null && tile != this;
    }
}