// RoadMovementLogic.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;
using static PathContainer;

/// <summary>
/// Esta clase <see cref="RoadMovementLogic" /> contiene la lógica para que el robot pequeño
/// se mueva por la carretera.
/// </summary>
public class RoadMovementLogic : MonoBehaviour
{
    /// <summary>
    /// El robot pequeño.
    /// </summary>
    [SerializeField] private MiniCharacter player;

    /// <summary>
    /// La velocidad de movimiento del robot.
    /// </summary>
    [SerializeField] private float speed = 20;

    /// <summary>
    /// Comprueba que se haya iniciado el movimiento.
    /// </summary>
    private bool movementStarted = false;

    /// <summary>
    /// Output final de la carretera.
    /// </summary>
    private RoadOutput finalOutput;

    /// <summary>
    /// Output de la pieza de carretera que estamos recorriendo.
    /// </summary>
    private RoadOutput nextOutput;

    /// <summary>
    /// El camino sobre el que se moverá el robot.
    /// </summary>
    private LTSpline track;

    /// <summary>
    /// Descriptor del tween.
    /// </summary>
    private LTDescr tweenDescr;

    /// <summary>
    /// Awake.
    /// </summary>
    private void Awake()
    {
        EventAggregator.Instance.Subscribe<MsgStartRoadMovement>(StartMovement);
        EventAggregator.Instance.Subscribe<MsgStopMovement>(StopMovement);
    }

    /// <summary>
    /// Recibe el mensaje de iniciar movimiento dado el input y el output de la carretera.
    /// </summary>
    /// <param name="msg">El mensaje<see cref="MsgStartRoadMovement"/>.</param>
    private void StartMovement(MsgStartRoadMovement msg)
    {
        //Resetemos todo
        movementStarted = false;
        finalOutput = msg.output;
        nextOutput = null;
        player.gameObject.SetActive(true);
        player.transform.position = msg.input.transform.position;
        track = null;
        tweenDescr = null;

        //Tomamos el camino que empieza en input
        if (StartNewPath(msg.input, out tweenDescr))
        {
            movementStarted = true;
        }
        else
        {
            Debug.LogError("No path available");
        }
    }

    /// <summary>
    /// Busca el siguiente camino posible y crea una LTSpline combinando los puntos nuevos y los anteriores.
    /// </summary>
    /// <param name="input">El input<see cref="RoadInput"/>.</param>
    /// <param name="ltDescr">El descriptor del tween como parámetro de salida<see cref="LTDescr"/>.</param>
    /// <returns>True si hay camino, false si no <see cref="bool"/>.</returns>
    private bool StartNewPath(RoadInput input, out LTDescr ltDescr)
    {
        Path path;
        RoadOutput outp;

        //Pedimos el camino correspondiente al input
        if (input.GetParentRoad().GetPathAndOutput(input, out path, out outp))
        {
            //Duplicamos el primer y el ultimo punto
            /*http://dentedpixel.com/LeanTweenDocumentation/classes/LTSpline.html */
            /*LTSpline ( pts )
            Defined in LeanTween.cs:2858
            Parameters:
            pts Vector3 Array
            A set of points that define the points the path will pass through (starting with starting
            control point, and ending with a control point)
            Note: The first and last item just define the angle of the end points, they are not actually
            used in the spline path itself. If you do not care about the angle you can just set the first
            two items and last two items as the same value.*/

            Vector3[] rawPath = new Vector3[path.points.Length + 2];

            rawPath[0] = path.points[0].position;
            for (int i = 0; i < path.points.Length; i++)
            {
                rawPath[i + 1] = path.points[i].position;
            }
            rawPath[rawPath.Length - 1] = path.points[path.points.Length - 1].position;

            //Si hemos recorrido un camino anteriormente, reemplazamos los dos primeros puntos por los finales
            //del camino anterior para evitar saltos
            if (track != null)
            {
                rawPath[0] = track.pts[track.pts.Length - 3];
                rawPath[1] = track.pts[track.pts.Length - 2];
            }

            //Creamos el camino
            track = new LTSpline(rawPath);

            //Ponemos como siguiente output la que resulta de procesar el camino nuevo
            nextOutput = outp;

            //Iniciamos el movimiento
            ltDescr = LeanTween.moveSpline(player.gameObject, track, track.distance / speed).setOrientToPath(true);

            return true;
        }

        ltDescr = null;
        return false;
    }

    /// <summary>
    /// Recibe el mensaje de parar el movimiento del robot.
    /// </summary>
    /// <param name="msg">El mensaje<see cref="MsgStopMovement"/>.</param>
    private void StopMovement(MsgStopMovement msg)
    {
        //Resetemos todo
        movementStarted = false;
        if (tweenDescr != null)
        {
            if (LeanTween.isTweening(tweenDescr.id))
            {
                LeanTween.cancel(tweenDescr.id);
            }
        }
    }

    /// <summary>
    /// Update.
    /// </summary>
    private void Update()
    {
        if (movementStarted)
        {
            //Comprueba si se ha acabado el proceso de tweening
            if (!LeanTween.isTweening(tweenDescr.id))
            {
                //Final de la carretera
                if (nextOutput == finalOutput)
                {
                    Debug.Log("End of the road!!");

                    movementStarted = false;
                    GameLogic.Instance.FinishedMinibotMovement = true;
                }
                else
                {
                    //Pedir mas camino
                    if (!StartNewPath((RoadInput)nextOutput.ConnectedTo, out tweenDescr))
                    {
                        movementStarted = false;
                        Debug.LogError("No path available");
                        GameLogic.Instance.FinishedMinibotMovement = true;
                    }
                }
            }
        }
    }
}