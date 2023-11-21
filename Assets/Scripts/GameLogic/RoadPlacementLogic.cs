// RoadPlacementLogic.cs
// Furious Koalas S.L.
// 2023

using System;
using System.Collections.Generic;
using UnityEngine;
using static LevelButtons;
using static RoadIO;

/// <summary>
/// La clase <see cref="RoadPlacementLogic" /> se encarga de formar la carretera apropiada
/// de acuerdo a las ordenes del usuario.
/// </summary>
public class RoadPlacementLogic : MonoBehaviour
{
    /// <summary>
    /// Señala la posición donde debe ponerse la primera carretera.
    /// </summary>
    [SerializeField] private Transform roadStartMarker;

    /// <summary>
    /// Flecha que marca qué carretera está seleccionada.
    /// </summary>
    [SerializeField] private SelectedOutputMarker selectedOutputMarker;

    /// <summary>
    /// Indica dentro de qué objeto deberán ponerse las carreteras.
    /// </summary>
    [SerializeField] private Transform roadParent;

    /// <summary>
    /// La carretera inicial.
    /// </summary>
    [SerializeField] private Road roadStart;

    /// <summary>
    /// El robot que se mueve por las carreteras.
    /// </summary>
    [SerializeField] private MiniCharacter minibot;

    /// <summary>
    /// Instancia de la clase que genera carreteras.
    /// </summary>
    [SerializeField] private RoadFactory roadFactory;

    /// <summary>
    /// Relaciona los botones con las acciones a tomar en consecuencia.
    /// </summary>
    private Dictionary<Buttons, Action> buttonActionsDictionary;

    /// <summary>
    /// IO seleccionada.
    /// </summary>
    private RoadIO selectedIO = null;

    /// <summary>
    /// Primer input de la carretera.
    /// </summary>
    private RoadIO firstInput = null;

    /// <summary>
    /// Primer input de la carretera.
    /// </summary>
    public RoadIO FirstInput
    {
        get
        {
            return firstInput;
        }
    }

    //Para escala 1 funcionaba bien 0.3, así que para escala 0.3 lo multiplico por ella
    /// <summary>
    /// La distancia máxima para no considerar un espacio entre dos carreteras como un hueco.
    /// </summary>
    private const float MAX_ACCEPTABLE_DISTANCE = 0.3f * 0.3f;

    /// <summary>
    /// IO seleccionada.
    /// </summary>
    public RoadIO SelectedIO { get => selectedIO; set => selectedIO = value; }

    /// <summary>
    /// Stack de acciones para poder deshacer las entradas del usuario.
    /// </summary>
    private Stack<RoadChanges> undoStack = new Stack<RoadChanges>();

    /// <summary>
    /// Instancia de la clase.
    /// </summary>
    private static RoadPlacementLogic roadPlacementLogic;

    /// <summary>
    /// Retorna la instancia de la clase.
    /// </summary>
    public static RoadPlacementLogic Instance
    {
        get
        {
            if (!roadPlacementLogic)
            {
                roadPlacementLogic = FindObjectOfType(typeof(RoadPlacementLogic)) as RoadPlacementLogic;

                if (!roadPlacementLogic)
                {
                    Debug.LogError("There needs to be one active RoadPlacementLogic script on a GameObject in your scene.");
                }
            }

            return roadPlacementLogic;
        }
    }

    /// <summary>
    /// Start.
    /// </summary>
    private void Start()
    {
        selectedOutputMarker.transform.parent = roadParent;
        roadStart.transform.parent = roadParent;
        roadParent.position = roadStartMarker.position;

        firstInput = roadStart.GetRoadIOByDirection(IODirection.Back)[0];
        firstInput.MoveRoadTo(roadStartMarker.position);
        this.selectedIO = roadStart.GetRoadIOByDirection(IODirection.Forward)[0];

        selectedOutputMarker.transform.position = this.selectedIO.transform.position;
        minibot.transform.parent = roadParent;
        minibot.transform.position = firstInput.transform.position;
        minibot.gameObject.SetActive(true);
        //Llenar el diccionario de funciones
        buttonActionsDictionary = new Dictionary<Buttons, Action>();
        buttonActionsDictionary.Add(Buttons.Action, DoAction);
        buttonActionsDictionary.Add(Buttons.Condition, DoCondition);
        buttonActionsDictionary.Add(Buttons.Jump, DoJump);
        buttonActionsDictionary.Add(Buttons.Loop, DoLoop);
        buttonActionsDictionary.Add(Buttons.Move, DoMove);
        buttonActionsDictionary.Add(Buttons.Play, DoPlay);
        buttonActionsDictionary.Add(Buttons.Restart, DoRestart);
        buttonActionsDictionary.Add(Buttons.TurnLeft, DoTurnLeft);
        buttonActionsDictionary.Add(Buttons.TurnRight, DoTurnRight);
        buttonActionsDictionary.Add(Buttons.Undo, DoUndo);
        buttonActionsDictionary.Add(Buttons.MapMenu, MapMenu);
    }

    /// <summary>
    /// Vuelve al menú de mapas.
    /// </summary>
    private void MapMenu()
    {
        ResetRoad();
        GameLogic.Instance.AddInputFromButton(Buttons.MapMenu);
    }

    /// <summary>
    /// La clase <see cref="RoadChanges" /> se usa para modelar los cambios en las carreteras.
    /// </summary>
    private class RoadChanges
    {
        /// <summary>
        /// Carreteras añadidas en esta acción.
        /// </summary>
        public List<Road> addedRoads = new List<Road>();

        /// <summary>
        /// Conexiones cambiadas en esta acción.
        /// </summary>
        public Dictionary<RoadIO, RoadIO> connectionsChanged = new Dictionary<RoadIO, RoadIO>();

        /// <summary>
        /// Botones añadidos en esta acción.
        /// </summary>
        public Tuple<NodeVerticalButton, VerticalButton> addedButton = null;

        /// <summary>
        /// IO seleccionada antes de esta acción.
        /// </summary>
        public RoadIO selectedIOBack = null;
    }

    /// <summary>
    /// Deshace las acciones del usuario.
    /// </summary>
    private void DoUndo()
    {
        if (undoStack.Count > 0)
        {
            RoadChanges thisChanges = undoStack.Pop();

            //Devolvemos al jugador las instrucciones correspondientes
            List<VerticalButton> addedButtons = new List<VerticalButton>();
            if (thisChanges.addedButton != null)
            {
                addedButtons.Add(thisChanges.addedButton.Item2);
            }

            RestoreInstructions(thisChanges.addedRoads, addedButtons);

            //Borramos las carreteras añadidas
            foreach (Road r in thisChanges.addedRoads)
            {
                Destroy(r.gameObject);
            }

            //Revertimos las conexiones
            foreach (KeyValuePair<RoadIO, RoadIO> entry in thisChanges.connectionsChanged)
            {
                entry.Key.ConnectedTo = entry.Value;
                // entry.Value.ConnectedTo = entry.Key;
            }

            //Borramos los botones añadidos
            if (thisChanges.addedButton != null)
            {
                thisChanges.addedButton.Item1.DestroyButton(thisChanges.addedButton.Item2);
            }

            //Se mueve la flecha de selección de io a la correspondiente posición
            if (thisChanges.selectedIOBack)
            {
                this.selectedIO = thisChanges.selectedIOBack;
                selectedOutputMarker.transform.position = this.SelectedIO.transform.position;
            }
            else
            {
                selectedOutputMarker.FindAndSelectClosestIO();
            }

            //Corrige las posiciones de las carreteras
            CorrectPositions(MAX_ACCEPTABLE_DISTANCE, firstInput);
        }
    }

    /// <summary>
    /// Ejecuta la acción correspondiente a un botón.
    /// </summary>
    /// <param name="buttonIndex">El botón pulsado <see cref="Buttons"/>.</param>
    public void AddInputFromButton(Buttons buttonIndex)
    {
        try
        {
            buttonActionsDictionary[buttonIndex]();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    /// <summary>
    /// Lógica del botón de acción.
    /// </summary>
    private void DoAction()
    {
        if (EnoughNumberOfInstrucions(Buttons.Action))
        {
            SpawnVerticalButton(Buttons.Action);
        }
    }

    /// <summary>
    /// Comprueba que haya un número suficiente de instrucciones para llevar a cabo
    /// una acción.
    /// </summary>
    /// <param name="button">El botón a comprobar <see cref="Buttons"/>.</param>
    /// <returns>True si hay suficientes, false si no <see cref="bool"/>.</returns>
    private bool EnoughNumberOfInstrucions(Buttons button)
    {
        /*if (Application.isEditor)
        {
            return true;
        }*/
        if (GameLogic.Instance.CurrentLevelData == null)
        {
            return false;
        }
        bool updateInstructions = false;

        switch (button)
        {
            case Buttons.Action:

                if (GameLogic.Instance.CurrentLevelData.availableInstructions.action > 0)
                {
                    GameLogic.Instance.CurrentLevelData.availableInstructions.action--;
                    updateInstructions = true;
                }
                break;

            case Buttons.Condition:

                if (GameLogic.Instance.CurrentLevelData.availableInstructions.condition > 0)
                {
                    GameLogic.Instance.CurrentLevelData.availableInstructions.condition--;
                    updateInstructions = true;
                }

                break;

            case Buttons.Jump:
                if (GameLogic.Instance.CurrentLevelData.availableInstructions.jump > 0)
                {
                    GameLogic.Instance.CurrentLevelData.availableInstructions.jump--;
                    updateInstructions = true;
                }

                break;

            case Buttons.Loop:

                if (GameLogic.Instance.CurrentLevelData.availableInstructions.loop > 0)
                {
                    GameLogic.Instance.CurrentLevelData.availableInstructions.loop--;
                    updateInstructions = true;
                }
                break;

            case Buttons.Move:

                if (GameLogic.Instance.CurrentLevelData.availableInstructions.move > 0)
                {
                    GameLogic.Instance.CurrentLevelData.availableInstructions.move--;
                    updateInstructions = true;
                }
                break;

            case Buttons.TurnLeft:

                if (GameLogic.Instance.CurrentLevelData.availableInstructions.turnLeft > 0)
                {
                    GameLogic.Instance.CurrentLevelData.availableInstructions.turnLeft--;
                    updateInstructions = true;
                }
                break;

            case Buttons.TurnRight:

                if (GameLogic.Instance.CurrentLevelData.availableInstructions.turnRight > 0)
                {
                    GameLogic.Instance.CurrentLevelData.availableInstructions.turnRight--;
                    updateInstructions = true;
                }
                break;
        }
        if (updateInstructions)
        {
            GameLogic.Instance.UpdateAvailableInstructions();
        }
        return updateInstructions;
    }

    /// <summary>
    /// Restable el número de instrucciones a un estado anterior.
    /// </summary>
    /// <param name="roads">Carreteras añadidas.</param>
    /// <param name="addedButton">Botón añadido.</param>
    private void RestoreInstructions(List<Road> roads, List<VerticalButton> addedButtons)
    {
        /*if (Application.isEditor)
        {
            return;
        }*/
        if (GameLogic.Instance.CurrentLevelData == null)
        {
            return;
        }

        bool updateInstructions = false;
        if (roads != null)
        {
            foreach (Road r in roads)
            {
                if (r is NodeIfIn)
                {
                    GameLogic.Instance.CurrentLevelData.availableInstructions.condition++;
                    updateInstructions = true;
                }
                if (r is NodeLoopIn)
                {
                    GameLogic.Instance.CurrentLevelData.availableInstructions.loop++;
                    updateInstructions = true;
                }
                if (r is NodeVerticalButton)
                {
                    NodeVerticalButton rVertical = (NodeVerticalButton)r;
                    VerticalButton[] currentButtons = rVertical.CurrentButtons;
                    if (currentButtons != null)
                    {
                        if (addedButtons == null)
                        {
                            addedButtons = new List<VerticalButton>();
                        }
                        foreach (VerticalButton vButton in currentButtons)
                        {
                            if (vButton != null)
                            {
                                addedButtons.Add(vButton);
                            }
                        }
                    }
                }
            }
        }
        if (addedButtons != null)
        {
            foreach (VerticalButton addedButton in addedButtons)
            {
                switch (addedButton.ButtonTypeE)
                {
                    case Buttons.Action:

                        GameLogic.Instance.CurrentLevelData.availableInstructions.action++;
                        updateInstructions = true;

                        break;

                    case Buttons.Jump:

                        GameLogic.Instance.CurrentLevelData.availableInstructions.jump++;
                        updateInstructions = true;

                        break;

                    case Buttons.Move:

                        GameLogic.Instance.CurrentLevelData.availableInstructions.move++;
                        updateInstructions = true;

                        break;

                    case Buttons.TurnLeft:

                        GameLogic.Instance.CurrentLevelData.availableInstructions.turnLeft++;
                        updateInstructions = true;

                        break;

                    case Buttons.TurnRight:

                        GameLogic.Instance.CurrentLevelData.availableInstructions.turnRight++;
                        updateInstructions = true;

                        break;
                }
            }
        }
        if (updateInstructions)
        {
            GameLogic.Instance.UpdateAvailableInstructions();
        }
    }

    /// <summary>
    /// Lógica del botón de condición. No se pueden poner
    /// condiciones dentro de condiciones.
    /// </summary>
    private void DoCondition()
    {
        if (EnoughNumberOfInstrucions(Buttons.Condition))
        {
            //No se pueden poner ifs dentro de ifs
            bool foundIf = false;
            if (this.selectedIO != null)
            {
                List<RoadIO> processedIO = new List<RoadIO>();
                Stack<RoadIO> ioToProc = new Stack<RoadIO>();

                foreach (RoadIO io in this.selectedIO.GetParentRoad().GetRoadIOByDirection(IODirection.Back))
                {
                    ioToProc.Push(io);
                }

                while (ioToProc.Count > 0 && !foundIf)
                {
                    RoadIO thisIO = ioToProc.Pop();
                    processedIO.Add(thisIO);
                    Debug.Log(thisIO.GetParentRoad().RoadIdentifier);
                    if (thisIO.GetParentRoad().RoadIdentifier.Contains("NodeIfIn"))
                    {
                        foundIf = true;
                    }
                    else if (!thisIO.GetParentRoad().RoadIdentifier.Contains("NodeIfOut"))
                    {
                        if (thisIO.ConnectedTo != null)
                        {
                            foreach (RoadIO io in thisIO.ConnectedTo.GetParentRoad().GetRoadIOByDirection(IODirection.Back))
                            {
                                if (!processedIO.Contains(io))
                                {
                                    ioToProc.Push(io);
                                }
                            }
                        }
                    }
                }
            }

            if (!foundIf)
            {
                RoadIO selectedIOBack = this.selectedIO;
                string[] ids = { "NodeIfIn", "NodeIfOut" };
                Road[] spawnedRoad;
                List<Road> extraRoads;
                Dictionary<RoadIO, RoadIO> oldConnections;
                if (SpawnRoads(ids, IODirection.Forward, out spawnedRoad, out extraRoads, out oldConnections))
                {
                    NewActionOnStack(spawnedRoad, extraRoads, oldConnections, selectedIOBack);
                }
            }
        }
    }

    /// <summary>
    /// Lógica del botón de salto.
    /// </summary>
    private void DoJump()
    {
        if (EnoughNumberOfInstrucions(Buttons.Jump))
        {
            SpawnVerticalButton(Buttons.Jump);
        }
    }

    /// <summary>
    /// Lógica del botón de bucle. No se pueden poner bucles dentro de condiciones.
    /// </summary>
    private void DoLoop()
    {
        if (EnoughNumberOfInstrucions(Buttons.Loop))
        {
            //No se pueden poner loops dentro de ifs
            bool foundIf = false;
            if (this.selectedIO != null)
            {
                List<RoadIO> processedIO = new List<RoadIO>();
                Stack<RoadIO> ioToProc = new Stack<RoadIO>();

                foreach (RoadIO io in this.selectedIO.GetParentRoad().GetRoadIOByDirection(IODirection.Back))
                {
                    ioToProc.Push(io);
                }

                while (ioToProc.Count > 0 && !foundIf)
                {
                    RoadIO thisIO = ioToProc.Pop();
                    processedIO.Add(thisIO);
                    Debug.Log(thisIO.GetParentRoad().RoadIdentifier);
                    if (thisIO.GetParentRoad().RoadIdentifier.Contains("NodeIfIn"))
                    {
                        foundIf = true;
                    }
                    else if (!thisIO.GetParentRoad().RoadIdentifier.Contains("NodeIfOut"))
                    {
                        if (thisIO.ConnectedTo != null)
                        {
                            foreach (RoadIO io in thisIO.ConnectedTo.GetParentRoad().GetRoadIOByDirection(IODirection.Back))
                            {
                                if (!processedIO.Contains(io))
                                {
                                    ioToProc.Push(io);
                                }
                            }
                        }
                    }
                }
            }

            if (!foundIf)
            {
                RoadIO selectedIOBack = this.selectedIO;
                string[] ids = { "NodeLoopIn", "NodeLoopOut" };
                Road[] spawnedRoad;
                List<Road> extraRoads;
                Dictionary<RoadIO, RoadIO> oldConnections;
                if (SpawnRoads(ids, IODirection.Forward, out spawnedRoad, out extraRoads, out oldConnections))
                {
                    NewActionOnStack(spawnedRoad, extraRoads, oldConnections, selectedIOBack);
                }
            }
        }
    }

    /// <summary>
    /// Conecta dos carreteras usando los ID de sus IOs.
    /// </summary>
    /// <param name="road1">La primera carretera <see cref="Road"/>.</param>
    /// <param name="road2">La segunda carretera <see cref="Road"/>.</param>
    /// <param name="ioR1_ioR2">Las conexiones a realizar <see cref="Dictionary{string, string}"/>.</param>
    /// <returns>True si se puede conectar, false si no <see cref="bool"/>.</returns>
    private bool ConnectRoads(in Road road1, in Road road2, in Dictionary<string, string> ioR1_ioR2)
    {
        bool success = true;

        //Conectamos una carretera a la otra
        foreach (KeyValuePair<string, string> entry in ioR1_ioR2)
        {
            RoadIO r1IO = road1.GetRoadIOByID(entry.Key);
            RoadIO r2IO = road2.GetRoadIOByID(entry.Value);

            if (r1IO != null && r2IO != null)
            {
                r1IO.ConnectedTo = r2IO;
                r2IO.ConnectedTo = r1IO;
            }
            else
            {
                Debug.LogError("Impossible to connect this roads");
                success = false;
                break;
            }
        }

        if (!success)
        {
            //Desconectamos todo
            foreach (KeyValuePair<string, string> entry in ioR1_ioR2)
            {
                RoadIO r1IO = road1.GetRoadIOByID(entry.Key);
                RoadIO r2IO = road2.GetRoadIOByID(entry.Value);

                if (r1IO != null)
                {
                    r1IO.ConnectedTo = null;
                }

                if (r2IO != null)
                {
                    r2IO.ConnectedTo = null;
                }
            }
        }

        return success;
    }

    /// <summary>
    /// Genera y conecta una serie de carreteras.
    /// </summary>
    /// <param name="ids">Lista con los IDs de las carreteras <see cref="string[]"/>.</param>
    /// <param name="direction">Dirección en la que apunta la primera carretera <see cref="IODirection"/>.</param>
    /// <param name="position">La posición en la que se tiene que colocar la primera carretera <see cref="Vector3"/>.</param>
    /// <param name="spawnedRoads">Parámetro de salida que incluye las carreteras generadas <see cref="Road[]"/>.</param>
    /// <returns>True si ha tenido éxito, false si no <see cref="bool"/>.</returns>
    private bool GenerateRoads(in string[] ids, in IODirection direction, in Vector3 position, out Road[] spawnedRoads)
    {
        spawnedRoads = new Road[ids.Length];

        if (ids.Length == 0)
        {
            return false;
        }

        //Generamos la primera
        Road spw;
        if (roadFactory.GetRoadByID(ids[0], out spw))
        {
            spawnedRoads[0] = Instantiate(spw, roadStart.transform.localPosition, roadStart.transform.rotation);
            Vector3 localRotationOnSpawn = spawnedRoads[0].transform.eulerAngles;
            spawnedRoads[0].transform.parent = roadParent;
            spawnedRoads[0].transform.eulerAngles = localRotationOnSpawn;
            spawnedRoads[0].GetRoadIOByDirection(RoadIO.GetOppositeDirection(direction))[0].MoveRoadTo(position);
        }
        else
        {
            return false;
        }

        for (int i = 0; i < ids.Length - 1; i++)
        {
            Road thisRoad = spawnedRoads[i];
            List<RoadIO> ioToMatch = thisRoad.GetRoadIOByDirection(direction);
            Road nextRoad;
            Dictionary<string, string> connectionsR_C;

            if (roadFactory.SpawnRoadByID(ids[i + 1], ioToMatch, out nextRoad, out connectionsR_C))
            {
                spawnedRoads[i + 1] = Instantiate(nextRoad, roadStart.transform.localPosition, roadStart.transform.rotation);
                Vector3 localRotationOnSpawn = spawnedRoads[i + 1].transform.eulerAngles;
                spawnedRoads[i + 1].transform.parent = roadParent;
                spawnedRoads[i + 1].transform.eulerAngles = localRotationOnSpawn;
                if (!ConnectRoads(spawnedRoads[i], spawnedRoads[i + 1], connectionsR_C))
                {
                    DestroyRoads(spawnedRoads);
                    return false;
                }
                else
                {
                    RoadIO io = spawnedRoads[i + 1].GetRoadIOByDirection(RoadIO.GetOppositeDirection(direction))[0];
                    io.MoveRoadTo(io.ConnectedTo.transform.position);
                }
            }
            else
            {
                DestroyRoads(spawnedRoads);
                return false;
            }
        }

        foreach (Road r in spawnedRoads)
        {
            r.transform.parent = roadParent;
        }

        return true;
    }

    /// <summary>
    /// Destruye una serie de carreteras.
    /// </summary>
    /// <param name="roads">Las carreteras a destruir <see cref="Road[]"/>.</param>
    private void DestroyRoads(Road[] roads)
    {
        for (int i = 0; i < roads.Length; i++)
        {
            if (roads[i] != null)
            {
                Destroy(roads[i]);
            }
        }
    }

    /// <summary>
    /// Genera una serie de carreteras teniendo en cuenta de rellenar con conectores los huecos
    /// que se puedan ir generando al crearlas.
    /// </summary>
    /// <param name="ids">Lista con los IDs de las carreteras <see cref="string[]"/>.</param>
    /// <param name="direction">Dirección en la que apunta la primera carretera <see cref="IODirection"/>.</param>
    /// <param name="spawnedRoads">Parámetro de salida que incluye las carreteras generadas <see cref="Road[]"/>.</param>
    /// <param name="extraSpawnedRoads">Parámetro de salida que incluye las carreteras generadas rellenando huecos <see cref="List{Road}"/>.</param>
    /// <param name="oldConnections">Las conexiones de las carreteras que se han roto al rellenar huecos<see cref="Dictionary{RoadIO, RoadIO}"/>.</param>
    /// <returns>True si ha tenido éxito, false si no <see cref="bool"/>.</returns>
    private bool SpawnRoads(in string[] ids, in IODirection direction, out Road[] spawnedRoads, out List<Road> extraSpawnedRoads, out Dictionary<RoadIO, RoadIO> oldConnections)
    {
        Vector3 pos = this.selectedIO != null ? this.selectedIO.transform.position : roadStartMarker.position;
        oldConnections = new Dictionary<RoadIO, RoadIO>();
        spawnedRoads = null;
        extraSpawnedRoads = new List<Road>();
        if (this.selectedIO != null && this.selectedIO == firstInput)
        {
            //NO SE PUEDE PONER AHÍ
            return false;
        }

        if (!GenerateRoads(ids, direction, pos, out spawnedRoads))
        {
            return false;
        }

        RoadIO roadIOL = null;
        RoadIO roadIOR = null;

        //Si hay io seleccionada
        if (this.selectedIO != null)
        {
            roadIOL = this.selectedIO;

            if (roadIOL.ConnectedTo != null)
            {
                roadIOR = this.selectedIO.ConnectedTo;
            }

            oldConnections.Add(roadIOL, roadIOL.ConnectedTo);
            roadIOL.ConnectedTo = spawnedRoads[0].GetRoadIOByDirection(RoadIO.GetOppositeDirection(direction))[0];

            if (roadIOR != null)
            {
                roadIOR.ConnectedTo = spawnedRoads[spawnedRoads.Length - 1].GetRoadIOByDirection(direction)[0];
            }

            roadIOL.ConnectedTo.MoveRoadTo(roadIOL.transform.position);
        }

        int numberOfPiecesGap = spawnedRoads.Length;

        //Hacer la magia

        //Guardo la dirección en la que se ha puesto esta carretera
        IODirection newRoadDir = direction;
        IODirection oppositeDirection = RoadIO.GetOppositeDirection(newRoadDir);
        //Cada vez que se pasa a una carretera por esa dirección, se suma un nivel
        //En dirección contraria se resta
        //Y en el resto se queda igual

        //Creo un diccionario de carreteras y el numero que tienen
        Dictionary<Road, int> roadAndValue = new Dictionary<Road, int>();

        //Lista de IO procesadas
        List<RoadIO> processedIO = new List<RoadIO>();
        List<Road> processedRoads = new List<Road>();

        //Stack de IO a procesar
        Stack<RoadIO> ioToProccess = new Stack<RoadIO>();

        //Añado el IO de la nueva carretera a la pila
        foreach (RoadIO r in spawnedRoads[spawnedRoads.Length - 1].GetAllIO())
        {
            if (r.ConnectedTo != null)
            {
                ioToProccess.Push(r);
            }
        }

        processedRoads.Add(spawnedRoads[spawnedRoads.Length - 1]);
        //Marco la nueva carretera con un 0
        roadAndValue.Add(spawnedRoads[spawnedRoads.Length - 1], 0);

        while (ioToProccess.Count > 0)
        {
            //Tomamos la io a procesar
            RoadIO currentIO = ioToProccess.Pop();

            //La añadimos a la lista de procesadas
            processedIO.Add(currentIO);

            if (currentIO.ConnectedTo != null)
            {
                RoadIO ConnectedTo = currentIO.ConnectedTo;
                int nextRoadLevel = roadAndValue[currentIO.GetParentRoad()];

                if (currentIO.Direction == newRoadDir)
                {
                    //La siguiente carretera suma
                    nextRoadLevel++;
                }
                else if (currentIO.Direction == oppositeDirection)
                {
                    //La siguiente carretera resta
                    nextRoadLevel--;
                }
                else
                {
                    ////Queda igual
                }

                //Si es menor que cero hemos encontrado un hueco
                if (nextRoadLevel <= 0)
                {
                    //Llenamos el hueco
                    List<RoadIO> currentRoadIO = new List<RoadIO>();
                    List<RoadIO> nextRoadIO = new List<RoadIO>();

                    foreach (RoadIO r in currentIO.GetParentRoad().GetRoadIOByDirection(currentIO.Direction))
                    {
                        if (Vector3.Distance(r.transform.position, r.ConnectedTo.transform.position) > MAX_ACCEPTABLE_DISTANCE)
                        {
                            oldConnections.Add(r, r.ConnectedTo);
                            currentRoadIO.Add(r);
                            nextRoadIO.Add(r.ConnectedTo);
                        }

                        if (currentRoadIO.Count > 0)
                        {
                            Road gap;

                            Dictionary<string, string> connectionsR1_Connector;
                            Dictionary<string, string> connectionsR2_Connector;

                            if (roadFactory.FillGapWithConnector(nextRoadIO, currentRoadIO, out gap, out connectionsR1_Connector, out connectionsR2_Connector))
                            {
                                string[] idsGap = new string[numberOfPiecesGap];
                                for (int i = 0; i < idsGap.Length; i++)
                                {
                                    idsGap[i] = gap.RoadIdentifier;
                                }

                                Road[] spanedRoads;
                                if (GenerateRoads(idsGap, nextRoadIO[0].Direction, nextRoadIO[0].transform.position, out spanedRoads))
                                {
                                    ConnectRoads(nextRoadIO[0].GetParentRoad(), spanedRoads[0], connectionsR1_Connector);
                                    ConnectRoads(currentRoadIO[0].GetParentRoad(), spanedRoads[spanedRoads.Length - 1], connectionsR2_Connector);
                                    foreach (Road newR in spanedRoads)
                                    {
                                        processedRoads.Add(newR);
                                        extraSpawnedRoads.Add(newR);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    //Si es mayor o igual que cero seguimos
                    if (!roadAndValue.ContainsKey(ConnectedTo.GetParentRoad()))
                    {
                        //Si no contiene esta carretera, la añadimos
                        roadAndValue.Add(ConnectedTo.GetParentRoad(), nextRoadLevel);
                    }

                    //Movemos la nueva carretera a su posicion
                    if (!processedRoads.Contains(ConnectedTo.GetParentRoad()))
                    {
                        ConnectedTo.MoveRoadTo(currentIO.transform.position);
                        processedRoads.Add(ConnectedTo.GetParentRoad());
                    }

                    //Añadimos nueva io
                    foreach (RoadIO r in ConnectedTo.GetParentRoad().GetAllIO())
                    {
                        if (r.ConnectedTo != null)
                        {
                            if (!processedIO.Contains(r))
                            {
                                ioToProccess.Push(r);
                            }
                        }
                    }
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Crea una carretera con botón (o modifica una existente si hay hueco).
    /// </summary>
    /// <param name="button">El botón a generar.</param>
    private void SpawnVerticalButton(Buttons button)
    {
        bool placed = false;

        if (this.selectedIO != null)
        {
            VerticalButton addedButton;
            if (this.selectedIO.GetParentRoad() is NodeVerticalButton)
            {
                NodeVerticalButton node = (NodeVerticalButton)this.selectedIO.GetParentRoad();

                if (node.AddButton(button.ToString(), this.selectedIO, out addedButton))
                {
                    RoadChanges c = new RoadChanges();
                    c.addedButton = new Tuple<NodeVerticalButton, VerticalButton>(node, addedButton);
                    c.selectedIOBack = this.selectedIO;
                    undoStack.Push(c);
                    placed = true;
                }
            }
            if (!placed && this.selectedIO.ConnectedTo != null)
            {
                if (this.selectedIO.ConnectedTo.GetParentRoad() is NodeVerticalButton)
                {
                    NodeVerticalButton node1 = (NodeVerticalButton)this.selectedIO.ConnectedTo.GetParentRoad();
                    if (node1.AddButton(button.ToString(), this.selectedIO.ConnectedTo, out addedButton))
                    {
                        RoadChanges c = new RoadChanges();
                        c.addedButton = new Tuple<NodeVerticalButton, VerticalButton>(node1, addedButton);
                        c.selectedIOBack = this.selectedIO;
                        undoStack.Push(c);
                        placed = true;
                    }
                }
            }
        }

        if (!placed)
        {
            RoadIO selectedIOBack = this.selectedIO;
            string[] ids = { "NodeVerticalButton" };
            Road[] spawnedRoad;
            List<Road> extraRoads;
            Dictionary<RoadIO, RoadIO> oldConnections;
            if (SpawnRoads(ids, IODirection.Forward, out spawnedRoad, out extraRoads, out oldConnections))
            {
                string[] args = { "activate", button.ToString() };
                spawnedRoad[0].ExecuteAction(args);
                NewActionOnStack(spawnedRoad, extraRoads, oldConnections, selectedIOBack);
            }
        }
    }

    /// <summary>
    /// Añade una nueva acción al stack de acciones.
    /// </summary>
    /// <param name="addedRoads">Las carreteras añadidas <see cref="Road[]"/>.</param>
    /// <param name="extraRoads">Las carreteras añadidas al cerrar huecos <see cref="List{Road}"/>.</param>
    /// <param name="connections">Las conexiones que se han hecho durante esta acción <see cref="Dictionary{RoadIO, RoadIO}"/>.</param>
    /// <param name="selectedIOback">La IO seleccionada antes de esta acción <see cref="RoadIO"/>.</param>
    private void NewActionOnStack(Road[] addedRoads, List<Road> extraRoads, Dictionary<RoadIO, RoadIO> connections, RoadIO selectedIOback)
    {
        RoadChanges rChanges = new RoadChanges();
        if (addedRoads != null)
        {
            foreach (Road rr in addedRoads)
            {
                rChanges.addedRoads.Add(rr);
            }
        }

        if (extraRoads != null)
        {
            foreach (Road rrl in extraRoads)
            {
                rChanges.addedRoads.Add(rrl);
            }
        }

        if (connections != null)
        {
            foreach (KeyValuePair<RoadIO, RoadIO> entry in connections)
            {
                rChanges.connectionsChanged.Add(entry.Key, entry.Value);
            }
        }

        rChanges.selectedIOBack = selectedIOback;

        undoStack.Push(rChanges);
    }

    /// <summary>
    /// Lógica del botón move.
    /// </summary>
    private void DoMove()
    {
        if (EnoughNumberOfInstrucions(Buttons.Move))
        {
            SpawnVerticalButton(Buttons.Move);
        }
    }

    /// <summary>
    /// Lógica del botón play.
    /// </summary>
    private void DoPlay()
    {
        if (selectedIO != null)
        {
            EventAggregator.Instance.Publish(new MsgDisableAllButtons());
            EventAggregator.Instance.Publish(new MsgEnableButton(Buttons.Restart));
            EventAggregator.Instance.Publish(new MsgEnableButton(Buttons.MapMenu));

            List<Road> allRoads = new List<Road>();
            Stack<Road> roadsToProccess = new Stack<Road>();

            RoadInput roadInput = null;
            RoadOutput roadOutput = null;

            roadsToProccess.Push(selectedIO.GetParentRoad());
            while (roadsToProccess.Count > 0)
            {
                Road r = roadsToProccess.Pop();

                foreach (RoadIO rIO in r.GetAllIO())
                {
                    if (rIO.ConnectedTo != null)
                    {
                        if (!allRoads.Contains(rIO.ConnectedTo.GetParentRoad()))
                        {
                            roadsToProccess.Push(rIO.ConnectedTo.GetParentRoad());
                        }
                    }
                    else
                    {
                        if (rIO is RoadInput)
                        {
                            roadInput = (RoadInput)rIO;
                        }
                        else
                        {
                            roadOutput = (RoadOutput)rIO;
                        }
                    }
                }

                if (!allRoads.Contains(r))
                {
                    allRoads.Add(r);
                }
            }

            Debug.Log("Number of roads: " + allRoads.Count);
            Debug.Log("RoadInput: " + roadInput.IOIdentifier);
            Debug.Log("RoadOutput: " + roadOutput.IOIdentifier);

            bool invalidRoad = false;
            foreach (Road r in allRoads)
            {
                if (!r.RoadReady())
                {
                    invalidRoad = true;
                    Debug.LogError(r.RoadIdentifier + " not ready");
                }
            }

            if (!invalidRoad)
            {
                //Bloquear las carreteras
                string[] lockArgs = { "lock" };
                foreach (Road r in allRoads)
                {
                    r.ExecuteAction(lockArgs);
                }
                selectedOutputMarker.gameObject.SetActive(false);
                EventAggregator.Instance.Publish(new MsgStartRoadMovement(roadInput, roadOutput));
            }
            else
            {
                InvalidRoad();
            }
        }
        else
        {
            InvalidRoad();
        }
    }

    /// <summary>
    /// Imprime un mensaje de carretera invalida.
    /// </summary>
    private void InvalidRoad()
    {
        Debug.LogError("Invalid Road");
    }

    /// <summary>
    /// Lógica del botón restart.
    /// </summary>
    public void DoRestart()
    {
        ResetRoad();
        GameLogic.Instance.AddInputFromButton(Buttons.Restart);
    }

    /// <summary>
    /// Resetea la carretera a su estado original.
    /// </summary>
    public void ResetRoad()
    {
        EventAggregator.Instance.Publish(new MsgEnableAllButtons());
        EventAggregator.Instance.Publish(new MsgStopMovement());

        while (undoStack.Count > 0)
        {
            RoadChanges c = undoStack.Pop();
            foreach (Road r in c.addedRoads)
            {
                Destroy(r.gameObject);
            }
        }

        selectedOutputMarker.transform.parent = roadParent;
        roadStart.transform.parent = roadParent;
        roadParent.position = roadStartMarker.position;

        firstInput = roadStart.GetRoadIOByDirection(IODirection.Back)[0];
        firstInput.MoveRoadTo(roadStartMarker.position);
        this.selectedIO = roadStart.GetRoadIOByDirection(IODirection.Forward)[0];

        selectedOutputMarker.transform.position = this.selectedIO.transform.position;
        selectedOutputMarker.gameObject.SetActive(true);
        minibot.transform.position = firstInput.transform.position;
        minibot.transform.rotation = new Quaternion();
        minibot.gameObject.SetActive(true);
    }

    /// <summary>
    /// Lógica del botón de girar a la izquierda.
    /// </summary>
    private void DoTurnLeft()
    {
        if (EnoughNumberOfInstrucions(Buttons.TurnLeft))
        {
            SpawnVerticalButton(Buttons.TurnLeft);
        }
    }

    /// <summary>
    /// Lógica del botón de girar a la derecha.
    /// </summary>
    private void DoTurnRight()
    {
        if (EnoughNumberOfInstrucions(Buttons.TurnRight))
        {
            SpawnVerticalButton(Buttons.TurnRight);
        }
    }

    /// <summary>
    /// Corrige las posiciones de las carreteras.
    /// </summary>
    /// <param name="maxAcceptableDistance">La distancia máxima aceptable entre dos carreteras<see cref="float"/>.</param>
    /// <param name="pivotIO">IO que se usa como pivote a la hora de hacer las comprobaciones <see cref="RoadIO"/>.</param>
    private void CorrectPositions(in float maxAcceptableDistance, RoadIO pivotIO)
    {
        if (pivotIO != null)
        {
            pivotIO.MoveRoadTo(roadStartMarker.transform.position);

            List<Road> processedRoads = new List<Road>();
            processedRoads.Add(pivotIO.GetParentRoad());

            Stack<RoadIO> ioToProc = new Stack<RoadIO>();

            RoadIO[] tmpe = pivotIO.GetParentRoad().GetAllIO();

            foreach (RoadIO rio in tmpe)
            {
                ioToProc.Push(rio);
            }

            while (ioToProc.Count > 0)
            {
                RoadIO toProc = ioToProc.Pop();
                RoadIO connectedTo = toProc.ConnectedTo;
                if (connectedTo != null)
                {
                    float distance = Vector3.Distance(toProc.transform.position, connectedTo.transform.position);
                    if (distance > maxAcceptableDistance)
                    {
                        connectedTo.MoveRoadTo(toProc.transform.position);
                    }

                    processedRoads.Add(connectedTo.GetParentRoad());

                    tmpe = connectedTo.GetParentRoad().GetAllIO();

                    foreach (RoadIO rio in tmpe)
                    {
                        if (rio.ConnectedTo != null)
                        {
                            if (!processedRoads.Contains(rio.ConnectedTo.GetParentRoad()))
                            {
                                ioToProc.Push(rio);
                            }
                        }
                    }
                }
            }
        }
    }
}