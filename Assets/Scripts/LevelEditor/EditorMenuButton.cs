using UnityEngine;

/// <summary>
/// Manda un mensaje <see cref="MsgEditorMenu"/> cuando el editor hace tap sobre una instancia de esta clase.
/// </summary>
public class EditorMenuButton : MonoBehaviour
{
    /// <summary>
    /// Manda un mensaje <see cref="MsgEditorMenu"/> cuando el editor hace tap sobre una instancia de esta clase.
    /// </summary>
    public void OnSelect()
    {
        EventAggregator.Instance.Publish<MsgEditorMenu>(new MsgEditorMenu());
    }
}