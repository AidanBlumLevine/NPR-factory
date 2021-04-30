using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileSelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject tile;
    public AnimationCurve selectScale, hoverScale;
    public Sprite selectedSprite;
    Sprite normalSprite;
    bool selected;
    Image image;
    int index;
    static int selectedIndex = -1;
    static float switchDelay;
    int speedScale = 6;

    void Start()
    {
        image = GetComponent<Image>();
        normalSprite = image.sprite;
        index = transform.GetSiblingIndex();
    }

    void Update()
    {
        if (selected && selectedIndex != index)
            Deselect();

        float raw = Input.GetAxis("Mouse ScrollWheel");
        if (selected && raw != 0 && switchDelay < 0)
        {
            switchDelay = .1f;
            int scr = (int)Mathf.Sign(raw);
            Deselect();
            transform.parent.GetChild((index - scr + transform.parent.childCount) % transform.parent.childCount).GetComponent<TileSelector>().Select();
        }
        if (index == 0)
            switchDelay -= Time.deltaTime;

        if (raw < 0 && selectedIndex == -1 && index == 0)
            Select();

        if (raw > 0 && selectedIndex == -1 && index == transform.parent.childCount - 1)
            Select();
    }

    void Deselect()
    {
        if (TileManager.Instance.selectedTile == tile)
            TileManager.Instance.selectedTile = null;
        if (selectedIndex == index)
            selectedIndex = -1;
        selected = false;
        image.sprite = normalSprite;
        StartCoroutine(Transition());
    }

    void Select()
    {
        TileManager.Instance.selectedTile = tile;
        selected = true;
        image.sprite = selectedSprite;
        selectedIndex = index;
        StartCoroutine(Transition());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!selected)
            Select();
        else
            Deselect();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!selected)
            StartCoroutine(Hover(true));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!selected)
            StartCoroutine(Hover(false));
    }

    IEnumerator Transition()
    {
        float time = 0;
        float end = selectScale.keys[selectScale.keys.Length - 1].time;
        while (time < end)
        {
            float trueTime = selected ? time : end - time;
            transform.localScale = Vector3.one * selectScale.Evaluate(trueTime);
            if (time < end / 2 && time + Time.deltaTime * speedScale > end / 2)
                image.sprite = selected ? selectedSprite : normalSprite;
            time += Time.deltaTime * speedScale;
            yield return null;
        }
        transform.localScale = Vector3.one * selectScale.Evaluate( selected ? time : end - time);

    }

    IEnumerator Hover(bool forward)
    {
        float time = 0;
        float end = hoverScale.keys[hoverScale.keys.Length - 1].time;
        while (time < end)
        {
            float trueTime = forward ? time : end - time;
            transform.localScale = Vector3.one * hoverScale.Evaluate(trueTime);
            time += Time.deltaTime * speedScale;
            yield return null;
        }
        transform.localScale = Vector3.one * hoverScale.Evaluate(forward ? time : end - time);
    }
}
