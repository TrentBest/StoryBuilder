window.storyBuilderCopy = async function (text) {
    if (!navigator.clipboard || !navigator.clipboard.writeText) {
        throw new Error("Clipboard API is unavailable in this context.");
    }

    await navigator.clipboard.writeText(text);
};
