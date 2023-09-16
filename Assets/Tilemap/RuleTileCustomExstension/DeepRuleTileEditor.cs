using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
    [CustomEditor(typeof(DeepRuleTile))]
    [CanEditMultipleObjects]
    public class DeepRuleTileEditor : RuleTileEditor
    {
        public Texture2D emptyIcon;
        public Texture2D otherTileIcon;

        public override void RuleOnGUI(Rect rect, Vector3Int position, int neighbor)
        {
            switch (neighbor)
            {
                case RuleTile.TilingRuleOutput.Neighbor.This:
                    GUI.DrawTexture(rect, arrows[GetArrowIndex(position)]);
                    break;
                case RuleTile.TilingRuleOutput.Neighbor.NotThis:
                    GUI.DrawTexture(rect, arrows[9]);
                    break;
                case DeepRuleTile.Neighbor.Empty:
                    GUI.DrawTexture(rect, emptyIcon);
                    break;
                case DeepRuleTile.Neighbor.OtherTile: 
                    GUI.DrawTexture(rect, otherTileIcon);
                    break;
                default:
                    var style = new GUIStyle();
                    style.alignment = TextAnchor.MiddleCenter;
                    style.fontSize = 10;
                    GUI.Label(rect, neighbor.ToString(), style);
                    break;
            }
        }
    }
}