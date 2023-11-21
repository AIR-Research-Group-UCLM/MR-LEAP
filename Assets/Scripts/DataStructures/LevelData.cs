// LevelData.cs
// Furious Koalas S.L.
// 2023

using System.Collections.Generic;

/// <summary>
/// La clase <see cref="LevelData" /> es la estructura que contiene todos los datos de un nivel.
/// </summary>
[System.Serializable]
public class LevelData
{
    /// <summary>
    /// Nombre del nivel.
    /// </summary>
    public string levelName;

    /// <summary>
    /// Tamaño del nivel.
    /// </summary>
    public List<int> levelSize;

    /// <summary>
    /// Posición inicial del jugador.
    /// </summary>
    public List<int> playerPos;

    /// <summary>
    /// Orientación inicial del jugador como un entero (0,1,2,3).
    /// </summary>
    public int playerOrientation;

    /// <summary>
    /// Meta del mapa.
    /// </summary>
    public List<int> goal;

    /// <summary>
    /// Guarda el número de instrucciones que le quedan al jugador.
    /// </summary>
    public AvailableInstructions availableInstructions;

    /// <summary>
    /// Los bloques y los items como una lista de ints.
    /// </summary>
    public List<int> mapAndItems;

    /// <summary>
    /// Hace una copia profunda del objeto.
    /// </summary>
    /// <returns>Retorna un clon del objeto.</returns>
    public LevelData Clone()
    {
        LevelData clone = new LevelData
        {
            levelName = this.levelName,
            playerOrientation = this.playerOrientation,
            levelSize = CloneListInt(this.levelSize),
            playerPos = CloneListInt(this.playerPos),
            goal = CloneListInt(this.goal),
            mapAndItems = CloneListInt(this.mapAndItems)
        };

        AvailableInstructions av = new AvailableInstructions
        {
            condition = this.availableInstructions.condition,
            loop = this.availableInstructions.loop,
            turnRight = this.availableInstructions.turnRight,
            turnLeft = this.availableInstructions.turnLeft,
            jump = this.availableInstructions.jump,
            move = this.availableInstructions.move,
            action = this.availableInstructions.action
        };

        clone.availableInstructions = av;

        return clone;
    }

    /// <summary>
    /// Clona una lista de enteros.
    /// </summary>
    /// <param name="original">La lista original.</param>
    /// <returns>La lista clonada.</returns>
    private List<int> CloneListInt(List<int> original)
    {
        List<int> clone = new List<int>();
        foreach (int i in original)
        {
            clone.Add(i);
        }

        return clone;
    }
}

/// <summary>
/// Guarda las instrucciones que tiene el jugador disponibles.
/// </summary>
[System.Serializable]
public class AvailableInstructions
{
    /// <summary>
    /// Condición.
    /// </summary>
    public int condition;

    /// <summary>
    /// Bucle.
    /// </summary>
    public int loop;

    /// <summary>
    /// Giro a la derecha.
    /// </summary>
    public int turnRight;

    /// <summary>
    /// Giro a la izquierda.
    /// </summary>
    public int turnLeft;

    /// <summary>
    /// Salto.
    /// </summary>
    public int jump;

    /// <summary>
    /// Movimiento.
    /// </summary>
    public int move;

    /// <summary>
    /// Acción.
    /// </summary>
    public int action;
}