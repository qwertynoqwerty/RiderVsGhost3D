using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Записывает траекторию движения объекта
/// </summary>
public class GhostRecorder : MonoBehaviour
{
    public List<Vector3> _positions = new List<Vector3>();
    public List<Quaternion> _rotations = new List<Quaternion>();
    
    private bool _isRecording;

    /// <summary>
    /// Начинает запись траектории
    /// </summary>
    public void Begin()
    {
        _positions.Clear();
        _rotations.Clear();
        _isRecording = true;
    }

    /// <summary>
    /// Останавливает запись траектории
    /// </summary>
    public void Stop()
    {
        _isRecording = false;
    }

    /// <summary>
    /// Обработчик записи траектории
    /// </summary>
    private void FixedUpdate()
    {
        if (!_isRecording) return;

        _positions.Add(transform.position);
        _rotations.Add(transform.rotation);
    }
}