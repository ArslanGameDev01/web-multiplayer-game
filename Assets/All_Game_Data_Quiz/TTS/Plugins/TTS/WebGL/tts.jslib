mergeInto(LibraryManager.library, 
{
  readTextAloud: function(textPtr, rate, pitch, langPtr) 
  {
    var text = UTF8ToString(textPtr);
    var lang = UTF8ToString(langPtr);
    text = text.replace(/___+/g, " blank ");
    text = text.replace(/---+/g, " blank ");

    var utterance = new SpeechSynthesisUtterance(text);
    utterance.lang = lang;
    utterance.pitch = pitch;
    utterance.rate = rate;

    var voices = window.speechSynthesis.getVoices();

    // Preferred female voices priority
    var preferred = voices.find(v => v.name.toLowerCase().includes("microsoft zira")) ||
                    voices.find(v => v.name.toLowerCase().includes("samantha")) || // iOS US female
                    voices.find(v => v.name.toLowerCase().includes("google uk english female")) ||
                    voices.find(v => v.name.toLowerCase().includes("female")) ||
                    voices.find(v => v.name.toLowerCase().includes("kathy")) || // iOS US female
                    voices.find(v => v.name.toLowerCase().includes("karen"));

    if (preferred) {
      utterance.voice = preferred;
      console.log("Selected female voice:", preferred.name);
    } else if (voices.length > 0) {
      utterance.voice = voices[0]; // fallback
      console.log("No female found, using default:", voices[0].name);
    } else {
      console.log("No voices available");
    }
    window.speechSynthesis.cancel();
    window.speechSynthesis.speak(utterance);
  },
  
  GetVoices: function() {
    var voices = window.speechSynthesis.getVoices();
    var voicesList = voices.map(voice => `${voice.name},${voice.lang}`).join(';');

    var bufferSize = lengthBytesUTF8(voicesList) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(voicesList, buffer, bufferSize);

    return buffer;
  }
});