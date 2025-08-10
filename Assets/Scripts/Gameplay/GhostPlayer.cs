using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Проигрывает записанную траекторию призрака
/// </summary>
public class GhostPlayer : MonoBehaviour
{
    private List<Vector3> _positions;
    private List<Quaternion> _rotations;
    private int _index;
    private bool _playing;

    /// <summary>
    /// Запускает воспроизведение траектории
    /// </summary>
    public void Begin(List<Vector3> pos, List<Quaternion> rot)
    {
        _positions = pos;
        _rotations = rot;
        _playing = _positions.Count > 0;
        _index = 0;

        if (_playing)
        {
            transform.SetPositionAndRotation(_positions[0], _rotations[0]);
        }
    }

    /// <summary>
    /// Воспроизведение траектории
    /// </summary>
    private void FixedUpdate()
    {
        if (!_playing) return;

        if (_index >= _positions.Count)
        {
            _playing = false;
            return;
        }

        transform.SetPositionAndRotation(_positions[_index], _rotations[_index]);
        _index++;
    }
}