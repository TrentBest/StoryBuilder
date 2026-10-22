window.storyBuilderCopy = async function (text) {
    if (!navigator.clipboard || !navigator.clipboard.writeText) {
        throw new Error("Clipboard API is unavailable in this context.");
    }

    await navigator.clipboard.writeText(text);
};

window.storyBuilderStorage = {
    get: function () {
        return window.localStorage.getItem("storybuilder.workspace.v1");
    },
    set: function (encodedWorkspace) {
        window.localStorage.setItem("storybuilder.workspace.v1", encodedWorkspace);
    },
    clear: function () {
        window.localStorage.removeItem("storybuilder.workspace.v1");
    }
};
