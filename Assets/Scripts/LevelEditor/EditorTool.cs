// EditorTool.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;
using static EditorLogic;

/// <summary>
/// Esta clase contiene el tipo de herramienta del editor e informa que se ha seleccionado cuando se hace click.
/// </summary>
[RequireComponent(typeof(Collider))]
public class EditorTool : MonoBehaviour
{
    /// <summary>
    /// Tipo de herramienta.
    /// </summary>
    [SerializeField] private EditorToolType toolType;

    /// <summary>
    /// Parámetro extra.
    /// </summary>
    [SerializeField] private int toolIdentifier;

    /// <summary>
    /// Manda un mensaje <see cref="MsgEditorToolSelected" /> al hacer tap en el objeto.
    /// </summary>
    public void OnSelect()
    {
        EventAggregator.Instance.Publish<MsgEditorToolSelected>(new MsgEditorToolSelected(toolType, toolIdentifier));
    }
}