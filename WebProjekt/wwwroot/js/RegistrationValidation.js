/*
    JavaScript ... Programiersprache des Browsers wird am Client-Pc ausgeführt

    jQuery ... ist eine JS-Bibliothek
        sie vereinfacht viele Aufgaben
           sie sorgt auch für die korrekte Ausführung in unterschiedlichen Browsern (früher wichtig)

    Zugriff auf ein id Element
        JS: document.getElementById("name")
        JQuery#: $("name") ... entspricht document.getElementById("name")
                 $´(".blue") ... Zugriff auf alle Elemente mit class="blue"
                 $("div") ... Zugriff auf alle Div.Tags

        $ ... entspricht dem JQuery-Objekt
        $() ... in den Klammern kann eine Methode/lamda-Ausdruck angegeben werden
            -> dieser wird ausgeführt, nachdem das html-Dokument komplett geladen ist
*/
alert("Trghjzgukhilt");

$(() => {   //Lamda Ausdruck verwenden
    //alert("Test");
    let formOK = true;
    

    //Lastname validieren
    // on("blur") ... der angegebene Lamdaausdruck wird beim Verlassen des Formularfeldes aufgerufen
    $("#Lastname").on("blur", () => {
        // .val() .. Inhalt des Eingabefeldes
        let ln = $("#Lastname").val().trim();


        if (!validateString(ln,2)) {
            //Fehlermeldung anzeigen
            // text("...")  ... es wird der angegebene Text ausgegeben
            // html("...")  .. hier können html-tags enthalten sein, diese werden interpretiert
            formOK = false;
            $("#lastnameError").text("Der Nachname muss mindestens 2 Zeichen lang sein!");
        }
        else {
            $("#lastnameError").text("");
        }
    });

    
    $("#Passwort").on("blur", () => {
        let ln = $("#Passwort").val().trim();
        validatePasswort(ln,2)
    });
    
    $("#Birthdate").on("blur", () => {
        let datum = $("#Birthdate").val();
        validateGeburtsdatum(datum);
    });
    
    $("#submitButton").on("click", (event) => {
        $("#Lastname").trigger("blur");
        //$("#Email").trigger("blur");
        $("#Passwort").trigger("blur");
        $("#Birthdate").trigger("blur");

        if (!formOK) {
            event.preventDefault();
        }

        formOK = true;
    });

    // Email: @ und zusätzlich, ob die Email-Adresse bereits vorhanden ist ( am Server))
    // müssen an denn server die eingebene Mail sender --> dieser soll uns z.B. True/false
    //zurückliefern, je nachdem, ob die Mail vorhanden ist oder nicht
    // fetch-API (JS) oder AJAX (JQuery) verwenden
    
    $("#Email").on("blur", () => {
        let mail = $("#Email").val().trim();

        
        if (!validateEmail(mail)) {
            formOK = false;
            $("#emailError").text("Die Email muss ein @ und einen . enthalten");
            return;
        } else {
            $("#emailError").text("");
        }

        
        fetch("/users/" + encodeURIComponent(mail), {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(response => {
                if (!response.ok) throw new Error("Fehler beim Serverabruf: " + response.status);
                return response.json();
            })
            .then(data => {
                if (data === true) {
                    $("#emailError").text("Diese E-Mail ist schon vorhanden");
                    formOK = false;
                } else {
                    $("#emailError").text(""); 
                }
            })
            .catch(error => {
                console.error("Fehler beim Abrufen der Daten:", error);
            });
    });
    




   

});


function validateString(text, minLength) {
    if (text.length < minLength) {
        formOK = false;
        return false;
    }
    return true;

}


function validatePasswort(text, minLength) {
    let fehler = [];

    if (text.length < minLength) {
        fehler.push("muss mindestens " + minLength + " Zeichen lang sein");
    }
    if (!/[a-z]/.test(text)) {
        fehler.push("muss einen Kleinbuchstaben enthalten");
    }
    if (!/[A-Z]/.test(text)) {
        fehler.push("muss einen Großbuchstaben enthalten");
    }
    if (!/[0-9]/.test(text)) {
        fehler.push("muss eine Zahl enthalten");
    }
    if (!/[^A-Za-z0-9]/.test(text)) {
        fehler.push("muss ein Sonderzeichen enthalten");
    }

    if (fehler.length === 0) {
        $("#passwortError").text("");
        return true;
    } else {
        formOK = false;
        let fehlertext = "Das Passwort " + fehler.join(" und ") + ".";
        $("#passwortError").text(fehlertext);
        return false;
    }
}
function validateGeburtsdatum(text) {
    if (text === "") {
        $("#geburtsdatumError").text("");
        return true;
    }

    let eingegebenesDatum = new Date(text);
    let heute = new Date();

    eingegebenesDatum.setHours(0, 0, 0, 0);
    heute.setHours(0, 0, 0, 0);

    if (eingegebenesDatum >= heute) {
        formOK = false;
        $("#geburtsdatumError").text("Das Geburtsdatum muss in der Vergangenheit liegen.");
        return false;
    } else {
        $("#geburtsdatumError").text("");
        return true;
    }
}

function validateEmail(text) {
    if (text.includes("@") && text.includes(".")) {
        return true;
    }
    return false;
}