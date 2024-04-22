function inputLimiter(e,allow) {
    var AllowableCharacters = '';

    if (allow == 'Letters'){AllowableCharacters=' ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz';}
    if (allow == 'Numbers'){AllowableCharacters='1234567890.';}
    if (allow == 'Date'){AllowableCharacters='1234567890-';}
    if (allow == 'NameCharacters'){AllowableCharacters=' ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-.\'';}
    if (allow == 'NameCharactersAndNumbers') { AllowableCharacters = '1234567890 ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-\''; }
    if (allow == 'NumbersAndCharacters') { AllowableCharacters = '1234567890 ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz'; }
    if (allow == 'multipleEmpNo') { AllowableCharacters = '1234567890 ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz,  '; }
    if (allow == 'AllCharacters') { AllowableCharacters = '1234567890 ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-\/    @$%().,:*_'; }

    var k = document.all?parseInt(e.keyCode): parseInt(e.which);
    if (k!=13 && k!=8 && k!=0){
        if ((e.ctrlKey==false) && (e.altKey==false)) {
        return (AllowableCharacters.indexOf(String.fromCharCode(k))!=-1);
        } else {
        return true;
        }
    } else {
        return true;
    }
} 


 function checkDate(sender,args) 
    { 
            if (sender._selectedDate < new Date())
            { 
                alert("You cannot select a day earlier than today!"); 
                sender._selectedDate = new Date();  
                sender._textbox.set_Value(sender._selectedDate.format(sender._format)) 
            } 
    }
    
    
    function openLink(objLink) { 
var w=window.open(objLink, 'popup', 'scrollbars=1,resizable=1,width=680,'
+'height=465,left=20,top=20'); 
//w.focus(); 
return false;
}


function openLink2(objLink) { 
var w=window.open(objLink, 'popup2', 'scrollbars=1,resizable=1,width=680,'
+'height=465,left=20,top=20'); 
w.focus(); 
return false;
}

function openLink3(objLink) {
    var w = window.open(objLink, 'popup', 'scrollbars=1,resizable=0,width=750,'
+ 'height=400,left=100,top=50');
    w.focus();
    return false;
}
    
    
//    var message="This function is not allowed here!";         
//    function clickIE4(){
//        if (event.button==2){ alert(message); return false; }
//    }
//    function clickNS4(e){
//        if (document.layers||document.getElementById&&!document.all){ 
//            if (e.which==2||e.which==3){ 
//                alert(message);
//                return false; 
//            } 
//        }
//    }
//    if (document.layers){ 
//        document.captureEvents(Event.MOUSEDOWN);
//        document.onmousedown=clickNS4;
//    } else if (document.all&&!document.getElementById){
//        document.onmousedown=clickIE4; 
//    } 
//    document.oncontextmenu=new Function("alert(message); return false") 
    
    
   function toUpper(txt)
    {
    document.getElementById(txt).value=document.getElementById(txt).value.toUpperCase();
    return true;
}

