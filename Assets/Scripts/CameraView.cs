using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum TouchType { NoTouch, Tap, LongPress};

public class CameraView : MonoBehaviour {

    private static int LEVEL_TIME = 90;
    
    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject loseMenu;
    public Text timeLeftText;

    private TargetableElement gazedElement;
    private CharacterController characterController;

    private float lastTouchTime;
    private float levelTimeLeft;

    private TargetableElement clickedElement;
    private Transform _originalParentTransform;
    private Vector3 _originalPosition;
    private Quaternion _originalRotation;

    private GameObject door;
    private GameObject key;
    private GameObject _tvScreen;

    private BookShelf _bookShelf;

    private MouseLook _mouseLook;
    private bool _isFirstClick;

    private  bool _booksAlreadySorted;

    private bool gameFinished;

	private float playerDistance;
	private AudioSource gameAudio;
	private bool canPlayAudio = true;
	private GameObject tv;

	private AudioSource source;
	private Vector3 previousVector;

	// Use this for initialization
	void Start () {
        Screen.lockCursor = true;
        characterController = GetComponent<CharacterController>();
        door = GameObject.FindGameObjectWithTag("Finish");
        key = GameObject.FindGameObjectWithTag("Key");
        key.SetActive(false);

        _tvScreen = GameObject.FindGameObjectWithTag("TvScreen");
        _tvScreen.SetActive(false);

        _bookShelf = GameObject.FindGameObjectWithTag("Bookshelf").GetComponent<BookShelf>();
        gameFinished = false;
        _booksAlreadySorted = false;

		tv = GameObject.Find ("FlatScreenTV");
		gameAudio = tv.GetComponent<AudioSource>();
		source = GetComponent<AudioSource> ();
//        source.Stop();

        _mouseLook = GetComponent<MouseLook>();

        levelTimeLeft = LEVEL_TIME;

        _isFirstClick = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (gameFinished) {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SetPauseActive(!pauseMenu.activeSelf);
        }

        if (pauseMenu.activeSelf) {
            return;
        }

        levelTimeLeft -= Time.deltaTime;
        timeLeftText.text = string.Format("{0}", (int) levelTimeLeft);

        if (gazedElement != null 
            && Vector3.Distance(transform.position, gazedElement.transform.position) < 3f) {
			source.Stop ();
            return;    
        }

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
        isGameOver();
	}

    public void SetPauseActive(bool isInPause) {
        pauseMenu.SetActive(isInPause);
        Screen.lockCursor = !isInPause;
        _mouseLook.SetPaused(isInPause);
        // timer
    }

	private void handleMusic() {
		playerDistance = Vector3.Distance (transform.position, tv.transform.position);
		if (playerDistance < 8 && tv.GetComponent<TvScreen>().child.activeSelf) {
			gameAudio.volume = (1 / playerDistance) * 0.012f; // adjust the volume to whatever you want
			if (canPlayAudio) {
				gameAudio.Play ();
				canPlayAudio = false;  // you can reset this in your code elsewhere... I did this just to make sure the audio only plays once.
			}
		} else {
			gameAudio.Stop ();
			canPlayAudio = true; 
		}
	}

    private void isGameOver() {
        if (gameFinished)
            return;

        float position = Vector3.Distance(door.transform.position, key.transform.position);
        gameFinished = position < 2;

        if (gameFinished) {
            source.Stop ();
            releaseTarget();
            winMenu.SetActive(true);
            GameFinished();
        }

        if (levelTimeLeft <= 0) {
            source.Stop ();
            levelTimeLeft = 0;
            loseMenu.SetActive(true);
            GameFinished();
        }
    }

    private void GameFinished() {
        Screen.lockCursor = false;
        _mouseLook.SetPaused(true);
    }

    public void onElementGaze(TargetableElement clickableElement) {
        gazedElement = clickableElement;
    }

    public void onElementGazeFinished() {
        gazedElement = null;
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

        _booksAlreadySorted = _bookShelf.isCombinationCorrect();
        if (_booksAlreadySorted) {
            _booksAlreadySorted = true;
            _bookShelf.getFirstBook().drop();
            StartCoroutine(WaitAndShowKey());
        }

        clickedElement = null;
    }

    private IEnumerator WaitAndShowKey() {
        yield return new WaitForSeconds(1.5f);
        key.SetActive(true);
    }

    private void move(Vector3 moveDirection) {
		previousVector = transform.position;
        Vector3 forwardVector = moveDirection * Time.deltaTime * 3;
        Vector3 futureVector = transform.position + forwardVector;
		if (Mathf.Pow (futureVector.x, 2) + Mathf.Pow (futureVector.z, 2) < Mathf.Pow (13f, 2)) {
			characterController.Move (forwardVector);
		}
		handleFootsteps ();
    }

	private void handleFootsteps() {
		if (!source.isPlaying && Vector3.Distance(transform.position, previousVector) > 0.06f) {
			source.Play ();
		} else if(source.isPlaying && Vector3.Distance(transform.position, previousVector) < 0.06f) {
			source.Stop();
		}
	}

}
