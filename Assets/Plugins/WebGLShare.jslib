mergeInto(LibraryManager.library, {
    ShareText: function(title, text, url) {
        title = UTF8ToString(title);
        text = UTF8ToString(text);
        url = UTF8ToString(url);

        if (navigator.share) {
            navigator.share({
                title: title,
                text: text,
                url: url
            }).then(() => {
                console.log('Share successful');
            }).catch((error) => {
                console.log('Sharing failed', error);
            });
        } else {
            console.log('Web Share API not supported');
        }
    },

    IsMobileBrowser: function() {
        return /iPhone|iPad|iPod|Android/i.test(navigator.userAgent);
    }
});