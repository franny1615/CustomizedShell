export abstract class LoadingIndicator {
    public static show() : void {
        document.getElementById('showLoadingIndicatorBtn')?.click();
    }

    public static hide() : void {
        var backOff = setInterval(() => {
            clearInterval(backOff);
            document.getElementById('hideLoadingIndicatorBtn')?.click();
        }, 1000);
    }
}