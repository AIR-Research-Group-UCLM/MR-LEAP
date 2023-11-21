// NodeVerticalButton.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;
using static PathContainer;

/// <summary>
/// Clase de la carretera con botones.
/// </summary>
public class NodeVerticalButton : Road
{
    /// <summary>
    /// Lista de botones.
    /// </summary>
    private VerticalButton[] buttonList;

    /// <summary>
    /// Input de la carretera.
    /// </summary>
    [SerializeField] private RoadInput rInput;

    /// <summary>
    /// Output de la carretera.
    /// </summary>
    [SerializeField] private RoadOutput rOutput;

    /// <summary>
    /// Botones en uso actualmente.
    /// </summary>
    private VerticalButton[] currentButtons = new VerticalButton[3];

    public VerticalButton[] CurrentButtons { get => currentButtons; }

    /// <summary>
    /// Start.
    /// </summary>
    private void Start()
    {
        buttonList = GetComponentsInChildren<VerticalButton>(true);
        Debug.Log("Button list length: " + buttonList.Length);
    }

    private void OnEnable()
    {
        if (buttonList == null || buttonList.Length == 0)
        {
            buttonList = GetComponentsInChildren<VerticalButton>(true);
        }
    }

    /// <summary>
    /// Destruye un botón.
    /// </summary>
    /// <param name="button">El botón <see cref="VerticalButton"/>.</param>
    /// <returns>True si ha podido destruirlo, false si no.</returns>
    public bool DestroyButton(in VerticalButton button)
    {
        for (int i = 0; i < currentButtons.Length; i++)
        {
            VerticalButton c = currentButtons[i];

            if (c != null)
            {
                if (c == button)
                {
                    Destroy(c.gameObject);
                    currentButtons[i] = null;
                    int numberOfButtons = NumberOfButtons();
                    Vector3 pos1 = Vector3.Lerp(rInput.transform.position, rOutput.transform.position, 0.5f);
                    Vector3 pos0 = Vector3.Lerp(rInput.transform.position, pos1, 0.5f);
                    Vector3 pos2 = Vector3.Lerp(pos1, rOutput.transform.position, 0.5f);
                    switch (numberOfButtons)
                    {
                        case 1:
                            for (int j = 0; j < currentButtons.Length; j++)
                            {
                                if (currentButtons[j] != null)
                                {
                                    VerticalButton t = currentButtons[j];
                                    currentButtons[j] = null;
                                    currentButtons[1] = t;
                                    t.transform.position = pos1;
                                    break;
                                }
                            }

                            break;

                        case 2:

                            VerticalButton button0 = null;
                            VerticalButton button2 = null;
                            for (int j = 0; j < currentButtons.Length; j++)
                            {
                                if (currentButtons[j] != null)
                                {
                                    if (button0 == null)
                                    {
                                        button0 = currentButtons[j];
                                    }
                                    else
                                    {
                                        button2 = currentButtons[j];
                                    }
                                    currentButtons[j] = null;
                                }
                            }

                            currentButtons[0] = button0;
                            currentButtons[2] = button2;
                            currentButtons[1] = null;

                            currentButtons[0].transform.position = pos0;
                            currentButtons[2].transform.position = pos2;
                            break;
                    }

                    return true;
                }
            }
        }

        foreach (VerticalButton c in currentButtons)
        {
        }
        return false;
    }

    /// <summary>
    /// Crea un botón si es posible.
    /// </summary>
    /// <param name="buttonName">Nombre del botón.</param>
    /// <param name="io">IO más cercana al botón.</param>
    /// <param name="spwButton">El botón creado.</param>
    /// <returns>True si lo ha podido crear, false si no.</returns>
    public bool AddButton(string buttonName, RoadIO io, out VerticalButton spwButton)
    {
        int numberOfButtons = NumberOfButtons();
        spwButton = null;
        if (numberOfButtons > 2 || (io != rInput && io != rOutput))
        {
            return false;
        }

        Vector3 pos1 = Vector3.Lerp(rInput.transform.position, rOutput.transform.position, 0.5f);
        Vector3 pos0 = Vector3.Lerp(rInput.transform.position, pos1, 0.5f);
        Vector3 pos2 = Vector3.Lerp(pos1, rOutput.transform.position, 0.5f);

        if (!SpawnButton(buttonName, out spwButton))
        {
            return false;
        }

        switch (numberOfButtons)
        {
            case 0:

                currentButtons[1] = spwButton;
                spwButton.transform.position = pos1;
                break;

            case 1:

                if (io == rInput)
                {
                    currentButtons[2] = currentButtons[1];
                    currentButtons[1] = null;
                    currentButtons[0] = spwButton;
                }
                else
                {
                    currentButtons[0] = currentButtons[1];
                    currentButtons[1] = null;
                    currentButtons[2] = spwButton;
                }

                currentButtons[2].transform.position = pos2;
                currentButtons[0].transform.position = pos0;
                break;

            case 2:
                if (io == rInput)
                {
                    currentButtons[1] = currentButtons[0];
                    currentButtons[0] = spwButton;
                }
                else
                {
                    currentButtons[1] = currentButtons[2];
                    currentButtons[2] = spwButton;
                }
                currentButtons[1].transform.position = pos1;
                currentButtons[2].transform.position = pos2;
                currentButtons[0].transform.position = pos0;

                break;

            default:
                return false;
        }
        return true;
    }

    /// <summary>
    /// Instancia un botón dado su nombre.
    /// </summary>
    /// <param name="bname">Nombre del botón.</param>
    /// <param name="button">El botón creado.</param>
    /// <returns>True si lo ha podido crear, false si no.</returns>
    private bool SpawnButton(in string bname, out VerticalButton button)
    {
        VerticalButton myButton = null;
        button = null;
        foreach (VerticalButton vbutton in buttonList)
        {
            if (vbutton.ButtonType.Equals(bname))
            {
                myButton = vbutton;
                break;
            }
        }

        if (myButton == null)
        {
            Debug.LogError("Not found: " + bname);
            return false;
        }
        else
        {
            button = Instantiate(myButton, myButton.transform.position, myButton.transform.rotation, this.transform);
            button.gameObject.SetActive(true);
            // button.transform.parent = this.transform;
            return true;
        }
    }

    /// <summary>
    /// Número actual de botones.
    /// </summary>
    /// <returns>El número de botones.</returns>
    private int NumberOfButtons()
    {
        int n = 0;

        for (int i = 0; i < currentButtons.Length; i++)
        {
            if (currentButtons[i] != null)
            {
                n++;
            }
        }

        Debug.Log("Number of buttons: " + n);
        return n;
    }

    /// <summary>
    /// Ejecuta una acción en base a una lista de argumentos.
    /// </summary>
    /// <param name="args">Los argumentos.</param>
    public override void ExecuteAction(in string[] args)
    {
        if (args.Length > 0)
        {
            switch (args[0])
            {
                case "activate":
                    if (args.Length > 1)
                    {
                        string buttonName = args[1];
                        bool foundButton = false;
                        if (buttonList == null || buttonList.Length == 0)
                        {
                            buttonList = GetComponentsInChildren<VerticalButton>(true);
                        }

                        foreach (VerticalButton vbutton in buttonList)
                        {
                            if (vbutton.ButtonType.Equals(buttonName))
                            {
                                vbutton.gameObject.SetActive(false);

                                VerticalButton spwB;
                                if (SpawnButton(buttonName, out spwB))
                                {
                                    foundButton = true;
                                    Vector3 pos1 = Vector3.Lerp(rInput.transform.position, rOutput.transform.position, 0.5f);
                                    currentButtons[1] = spwB;
                                    NumberOfButtons();
                                    spwB.transform.position = pos1;
                                }
                            }
                            else
                            {
                                vbutton.gameObject.SetActive(false);
                            }
                        }

                        if (foundButton)
                        {
                            Debug.Log("Button " + buttonName + " activated.");
                        }
                        else
                        {
                            Debug.Log("Button " + buttonName + " not found.");
                        }
                    }

                    break;

                case "lock":
                    foreach (VerticalButton vbutton in buttonList)
                    {
                        vbutton.Lock();
                    }
                    Debug.Log("Buttons locked.");
                    break;

                case "unlock":
                    foreach (VerticalButton vbutton in buttonList)
                    {
                        vbutton.Unlock();
                        Debug.Log("Buttons unlocked.");
                    }
                    break;

                default:
                    Debug.LogWarning("Undefined action: " + args[0]);
                    break;
            }
        }
    }

    /// <summary>
    /// Dado un input retorna el camino que inicia en él y el output en el que termina.
    /// </summary>
    /// <param name="input">El input <see cref="RoadInput"/>.</param>
    /// <param name="path">El camino <see cref="Path"/>.</param>
    /// <param name="output">El output <see cref="RoadOutput"/>.</param>
    /// <returns>True si hay camino, false si no.</returns>
    public override bool GetPathAndOutput(in RoadInput input, out Path path, out RoadOutput output)
    {
        if (input == rInput)
        {
            GetPathByName("Path", out path);
            output = path.ioEnd;
            return true;
        }

        path = new Path();
        output = null;
        return false;
    }
}