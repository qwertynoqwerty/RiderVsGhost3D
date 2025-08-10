using System.Collections;
using System.Collections.Generic;
using Ashsvp;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Менеджер гонки
/// </summary>
public class RaceManager : MonoBehaviour
{
    [Header("Машина")] 
    [SerializeField] private GameObject _car;
    [SerializeField] private Transform _startPoint;
    [SerializeField] private AudioSource _audio;

    [Header("Призрак")]
    [SerializeField] private GameObject _ghostObject;
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _countdownText;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private TextMeshProUGUI _runText;
    [SerializeField] private GameObject _finishPanel;
    
    
    private int _countdownSeconds = 3;
    
    private Transform _vehicleRoot;
    private Rigidbody _vehicleRb;
    private SimcadeVehicleController _vehicleController;
    private GhostRecorder _recorder;
    private GhostPlayer _ghostPlayer;

    private static List<Vector3> _ghostPositions;
    private static List<Quaternion> _ghostRotations;

    private bool _isRun2;
    private bool _running;
    private float _raceTime;

    private void Awake()
    {
        // Получение ссылок
        _vehicleRoot = _car.GetComponent<Transform>();
        _vehicleRb = _car.GetComponent<Rigidbody>();
        _vehicleController = _car.GetComponent<SimcadeVehicleController>();
        _recorder = _car.GetComponent<GhostRecorder>();
        _ghostPlayer = _ghostObject.GetComponent<GhostPlayer>();

        // Определение номера заезда
        _isRun2 = _ghostPositions != null && _ghostPositions.Count > 0;

        // Обновление UI
        if (_isRun2)
        {
            _runText.text = "Заезд: 2/2";
        }
        else
        {
            _runText.text = "Заезд: 1/2";
        }
    }

    private void Start()
    {
        _vehicleController.enabled = false;
        _vehicleRb.linearVelocity = Vector3.zero;
        _vehicleRb.angularVelocity = Vector3.zero;
        _vehicleRoot.SetPositionAndRotation(_startPoint.position, _startPoint.rotation);        
        
        StartCoroutine(CountdownThenGo());
    }

    /// <summary>
    /// Отсчет до старта гонки
    /// </summary>
    private IEnumerator CountdownThenGo()
    {
        // Отсчет
        for (int i = _countdownSeconds; i > 0; i--)
        {
            if (_countdownText)
                _countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        // Обновление UI
        if (_countdownText) _countdownText.text = "GO!";
        yield return new WaitForSeconds(0.5f);
        if (_countdownText) _countdownText.text = "";

        // Старт
        _running = true;
        _vehicleController.enabled = true;
        _raceTime = 0f;

        // Включение призрака
        if (_isRun2)
        {
            _ghostObject.SetActive(true);
            _ghostPlayer.Begin(_ghostPositions, _ghostRotations);
        }

        // Запись
        if (!_isRun2)
        {
            _recorder.Begin();
        }
    }

    /// <summary>
    /// Таймер гонки
    /// </summary>
    private void Update()
    {
        if (!_running) return;

        _raceTime += Time.deltaTime;
        if (_timerText)
        {
            _timerText.text = $"{(int)(_raceTime / 60):00}:{_raceTime % 60:00.000}";
        }
    }

    /// <summary>
    /// Обработчик пересечение финиша
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _running = false;
            _vehicleController.enabled = false;

            // Сохранение траектории и перезапуск сцены
            if (!_isRun2)
            {
                _recorder.Stop();
                _ghostPositions = _recorder._positions;
                _ghostRotations = _recorder._rotations;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            // Очистка данных и показания панели финиша
            else
            {
                _ghostPositions = null;
                _ghostRotations = null;
                _audio.Stop();
                _runText.gameObject.SetActive(false);
                _timerText.gameObject.SetActive(false);
                _ghostObject.SetActive(false);
                _finishPanel.SetActive(true);
            }
        }
    }
}
