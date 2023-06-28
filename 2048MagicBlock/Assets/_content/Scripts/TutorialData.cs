using UnityEngine.UI;
using System;
using UnityEngine;

[Serializable]
public struct TutorialMoveTip
{
    public Vector2Int src;
    public Vector2Int dst;
}

[Serializable]
public struct TutorialData
{
    public Image hand;
    public Image FadeBackground;
    public Image cellPreview;

    public AnimationCurve moveCurve;

    public TutorialMoveTip[] tutorialMoveTips;

    public TutorialMoveTip GetMoveTipData()
    {
        return tutorialMoveTips[step];
    }

    [HideInInspector]
    public Vector2Int sourceMoveCell;
    [HideInInspector]
    public Vector2Int destinationMoveCell;
    [HideInInspector]
    public int step;

    public Image cellHighlightPrefab;

    [HideInInspector]
    public Image cellHighlight2;
}
