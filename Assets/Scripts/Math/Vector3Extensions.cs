// Vector3Extensions.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;

/// <summary>
/// Clase de extensión del Vector3 de unity que realiza las operaciones de forma más rápida.
/// Sinceramente no la he usado porque se me olvida que la hice y hacer 'vector1 + vector2' es más
/// cómodo que 'vector1.FastAdd(vector2)' pero la dejo por si me sirve en el trabajo futuro de este
/// proyecto o en otro que lleve a cabo.
/// </summary>
public static class Vector3Extensions
{
    /// <summary>
    /// Suma dos vectores.
    /// </summary>
    /// <param name="a">Vector a.</param>
    /// <param name="b">Vector b.</param>
    /// <returns>Retorna el resultado de la operación.</returns>
    public static Vector3 FastAdd(in this Vector3 a, in Vector3 b)
    {
        Vector3 result;
        result.x = a.x + b.x;
        result.y = a.y + b.y;
        result.z = a.z + b.z;
        return result;
    }

    /// <summary>
    /// Hace la operación a - b.
    /// </summary>
    /// <param name="a">Vector a.</param>
    /// <param name="b">Vector b.</param>
    /// <returns>Retorna el resultado de la operación.</returns>
    public static Vector3 FastSub(in this Vector3 a, in Vector3 b)
    {
        Vector3 result;
        result.x = a.x - b.x;
        result.y = a.y - b.y;
        result.z = a.z - b.z;
        return result;
    }

    /// <summary>
    /// Cambia de signo los componentes del vector.
    /// </summary>
    /// <param name="a">Vector a.</param>
    /// <returns>Retorna el resultado de la operación.</returns>
    public static Vector3 FastNeg(in this Vector3 a)
    {
        Vector3 result;
        result.x = -a.x;
        result.y = -a.y;
        result.z = -a.z;
        return result;
    }

    /// <summary>
    /// Multiplica un vector por un número.
    /// </summary>
    /// <param name="a">Vector a.</param>
    /// <param name="d">El número.</param>
    /// <returns>Retorna el resultado de la operación.</returns>
    public static Vector3 FastMult(in this Vector3 a, in float d)
    {
        Vector3 result;
        result.x = a.x * d;
        result.y = a.y * d;
        result.z = a.z * d;
        return result;
    }

    /// <summary>
    /// Divide un vector entre un número.
    /// </summary>
    /// <param name="a">Vector a.</param>
    /// <param name="d">El número.</param>
    /// <returns>Retorna el resultado de la operación.</returns>
    public static Vector3 FastDiv(in this Vector3 a, in float d)
    {
        Vector3 result;
        result.x = a.x / d;
        result.y = a.y / d;
        result.z = a.z / d;
        return result;
    }
}