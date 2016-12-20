using UnityEngine;
using System.Collections;

public class BookShelf : MonoBehaviour {

    private Book[] books;
    private Book firstBook;

	// Use this for initialization
	void Start () {
        books = new Book[8];
        int bookIndex = 0;
        int children = transform.childCount;
        for (int i = 0; i < children; i++) {
            Book book = transform.GetChild(i).GetComponent<Book>();
            if (book != null) {
                books[bookIndex++] = book;
                if (book.correctPosition == 0) {
                    firstBook = book;
                }
            }
        }
	}
	
    public bool isCombinationCorrect() {
        int correctBooks = 0;
        for (int i = 0; i < 8; i++) {
            if (books[i].actualPosition == books[i].correctPosition) {
                correctBooks++;
            }
        }
        return correctBooks >= 5;
    }

    public Book getFirstBook() {
        return firstBook;
    }

}
