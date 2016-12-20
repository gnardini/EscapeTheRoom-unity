using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum TouchType { NoTouch, Tap, LongPress};

public class CameraView : MonoBehaviour {

    private static float LEVEL_TIME = 90f;
    
    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject loseMenu;
    public Text timeLeftText;

    public GameObject _tvScreen;
    public GameObject key;
    public GameObject door;

    private CharacterController _characterController;
    private float _levelTimeLeft;

    private TargetableElement clickedElement;
    private Transform _originalParentTransform;
    private Vector3 _originalPosition;
    private Quaternion _originalRotation;

    public BookShelf bookShelf;

    private MouseLook _mouseLook;

    private  bool _booksAlreadySorted;

    private bool _gameFinished;

	private float playerDistance;
	public AudioSource gameAudio;
	private bool canPlayAudio = true;

	private AudioSource _source;
	private Vector3 previousVector;

	public void Start () {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _characterController = GetComponent<CharacterController>();
        _source = GetComponent<AudioSource> ();
        _mouseLook = GetComponent<MouseLook>();

        _gameFinished = false;
        _booksAlreadySorted = false;
        _levelTimeLeft = LEVEL_TIME;
	}
	
	void Update () {
        if (_gameFinished) {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SetPauseActive(!pauseMenu.activeSelf);
        }

        if (pauseMenu.activeSelf) {
            return;
        }

        transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);

        _levelTimeLeft -= Time.deltaTime;
        timeLeftText.text = string.Format("{0}", (int) _levelTimeLeft);

        previousVector = transform.position;
        if (Input.GetKey(KeyCode.W)) {
            move (transform.forward);
        }
        if (Input.GetKey(KeyCode.S)) {
            move (-transform.forward);
        }
        if (Input.GetKey(KeyCode.A)) {
            move (-transform.right);
        }
        if (Input.GetKey(KeyCode.D)) {
            move (transform.right);
        }

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) {
            var ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                TargetableElement element = hit.collider.gameObject.GetComponent<TargetableElement>();
                if (element != null) {
                    element.onClicked();
                    onElementClicked(element);   
                } else {
                    releaseTarget();  
                }
            } else {
                releaseTarget();
            }
        }

        transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);
		handleMusic();
        handleFootsteps();
        isGameOver();
	}

    public void SetPauseActive(bool isInPause) {
        pauseMenu.SetActive(isInPause);
        Cursor.lockState = isInPause ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isInPause;
        _mouseLook.SetPaused(isInPause);
    }

	private void handleMusic() {
        playerDistance = Vector3.Distance (transform.position, _tvScreen.transform.position);
        if (playerDistance < 8 && _tvScreen.activeSelf) {
			gameAudio.volume = (1 / playerDistance) * 0.012f; 
			if (canPlayAudio) {
				gameAudio.Play ();
				canPlayAudio = false; 
			}
		} else {
			gameAudio.Stop ();
			canPlayAudio = true; 
		}
	}

    private void isGameOver() {
        if (_gameFinished)
            return;

        float position = Vector3.Distance(door.transform.position, key.transform.position);
        _gameFinished = position < 2;

        if (_gameFinished) {
            _source.Stop ();
            releaseTarget();
            winMenu.SetActive(true);
            int lastBestLevel = PlayerPrefs.GetInt("max_level", 0);
            PlayerPrefs.SetInt("max_level", Mathf.Max(lastBestLevel, 1));
            GameFinished();
        }

        if (_levelTimeLeft <= 0) {
            _source.Stop ();
            _levelTimeLeft = 0;
            loseMenu.SetActive(true);
            GameFinished();
        }
    }

    private void GameFinished() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _mouseLook.SetPaused(true);
    }

    public void onElementClicked(TargetableElement element) {
        
        if (Vector3.Distance(transform.position, element.transform.position) > element.clickRange()) {
            return;
        }

        if (element is Book && _booksAlreadySorted) {
            return;
        }

        if (element is Book && clickedElement is Book) {
            swapBooks(element as Book);
            return;
        } 

        if (element is TvScreen) {
            if (clickedElement != null && clickedElement is Remote) {
                ((TvScreen)element).turnOnScreen();
                releaseTarget();
            }
            return;
        }

        if (clickedElement != null) {
            releaseTarget();
        }
        _originalParentTransform = element.transform.parent;
        _originalPosition = element.transform.position;
        _originalRotation = element.transform.rotation;
        clickedElement = element;
        element.transform.parent = transform;
        element.transform.localPosition = new Vector3(.5f, -.6f, 1f);
    }

    private void releaseTarget() {
        if (clickedElement == null) {
            return;
        }
        clickedElement.transform.parent = _originalParentTransform;
        clickedElement.transform.position = _originalPosition;
        clickedElement.transform.rotation = _originalRotation;
        clickedElement = null;
    }

    private void swapBooks(Book newBook) {
        Book oldBook = clickedElement as Book;

        oldBook.transform.parent = newBook.transform.parent;
        oldBook.transform.position = newBook.transform.position;
        oldBook.transform.rotation = newBook.transform.rotation;

        newBook.transform.parent = _originalParentTransform;
        newBook.transform.position = _originalPosition;
        newBook.transform.rotation = _originalRotation;

        int oldBookPosition = oldBook.actualPosition;
        oldBook.actualPosition = newBook.actualPosition;
        newBook.actualPosition = oldBookPosition;

        _booksAlreadySorted = bookShelf.isCombinationCorrect();
        if (_booksAlreadySorted) {
            _booksAlreadySorted = true;
            bookShelf.getFirstBook().drop();
            StartCoroutine(WaitAndShowKey());
        }

        clickedElement = null;
    }

    private IEnumerator WaitAndShowKey() {
        yield return new WaitForSeconds(1.5f);
        key.SetActive(true);
    }

    private void move(Vector3 moveDirection) {
        Vector3 forwardVector = moveDirection * Time.deltaTime * 3;
        Vector3 futureVector = transform.position + forwardVector;
		if (Mathf.Pow (futureVector.x, 2) + Mathf.Pow (futureVector.z, 2) < Mathf.Pow (13f, 2)) {
			_characterController.Move (forwardVector);
		}
    }

	private void handleFootsteps() {
		if (!_source.isPlaying && Vector3.Distance(transform.position, previousVector) > 0.00001f) {
			_source.Play ();
        } else if(_source.isPlaying && Vector3.Distance(transform.position, previousVector) < 0.00001f) {
			_source.Stop();
		}
	}

}
