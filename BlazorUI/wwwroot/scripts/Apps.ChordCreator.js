// JavaScript source code
var Ab = { selected: false, clicked: false };
var An = { selected: false, clicked: false };
var As = { selected: false, clicked: false };

var Bb = { selected: false, clicked: false };
var Bn = { selected: false, clicked: false };
var Bs = { selected: false, clicked: false };

var Cb = { selected: false, clicked: false };
var Cn = { selected: false, clicked: false };
var Cs = { selected: false, clicked: false };

var Db = { selected: false, clicked: false };
var Dn = { selected: false, clicked: false };
var Ds = { selected: false, clicked: false };

var Eb = { selected: false, clicked: false };
var En = { selected: false, clicked: false };
var Es = { selected: false, clicked: false };

var Fb = { selected: false, clicked: false };
var Fn = { selected: false, clicked: false };
var Fs = { selected: false, clicked: false };

var Gb = { selected: false, clicked: false };
var Gn = { selected: false, clicked: false };
var Gs = { selected: false, clicked: false };

var AllNotes = { selected: false };

window.activateChords = (() => {
    document.addEventListener('keypress', function (event) {
        event.preventDefault();
        if (event.keyCode === 13) {
            HearChord();
        } else if (event.keyCode > 64 && event.keyCode < 72) {
            alert(event.keyCode);
            NoteClick(String.fromCharCode(event.keyCode + 32) + "n");
            alert(event.key + "n");
        }
    });
});

function NoteClick(note) {
    var selnotes = document.getElementById("selnotes");
    var button = document.getElementById(note);
    if (eval(note + ".selected === false")) {
        eval(note + ".selected = true");
        if (AllNotes.selected === false) {
            selnotes.innerHTML = "";
            AllNotes.selected = true;
        }
        selnotes.innerHTML += note.replace("s", "#") + " ";
        button.attributes["class"] = "btn btn-secondary";
    } else {
        eval(note + ".selected = false");
        selnotes.innerHTML = selnotes.innerHTML.replace(note.replace("s", "#") + " ", "");
        if (selnotes.innerHTML === "") {
            selnotes.innerHTML = "No notes selected yet.";
            AllNotes.selected = false;
        }
        button.attributes["class"] = "btn btn-secondary";
    }
}

function ClearAll() {
    document.getElementById("selnotes").innerHTML = "No notes selected yet.";
    SettoFalse("A");
    SettoFalse("B");
    SettoFalse("C");
    SettoFalse("D");
    SettoFalse("E");
    SettoFalse("F");
    SettoFalse("G");
    AllNotes.selected = false;
}
function SettoFalse(key) {
    eval(key + "b.selected = false");
    document.getElementById(key + "b").attributes["class"] = "btn btn-outline-secondary";
    eval(key + "n.selected = false");
    document.getElementById(key + "n").attributes["class"] = "btn btn-outline-secondary";
    eval(key + "s.selected = false");
    document.getElementById(key + "s").attributes["class"] = "btn btn-outline-secondary";
}
function HearChord() {
    if (AllNotes.selected === true) {
        PlaySound("A");
        PlaySound("B");
        PlaySound("C");
        PlaySound("D");
        PlaySound("E");
        PlaySound("F");
        PlaySound("G");
    }
}
function PlaySound(key) {
    var audio;
    var path = "/audio/";
    if (eval(key + "b.selected === true")) {
        audio = new Audio(path + key + "b.wav");
        audio.play();
    }
    if (eval(key + "n.selected === true")) {
        audio = new Audio(path + key + "n.wav");
        audio.play();
    }
    if (eval(key + "s.selected === true")) {
        audio = new Audio(path + key + "s.wav");
        audio.play();
    }
}