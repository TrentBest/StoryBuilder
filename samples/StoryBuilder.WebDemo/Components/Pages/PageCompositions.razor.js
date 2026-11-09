export function getNormalizedPoint(element, clientX, clientY) {
    const bounds = element.getBoundingClientRect();
    if (!bounds.width || !bounds.height) return { x: 0.5, y: 0.5 };
    return {
        x: Math.min(1, Math.max(0, (clientX - bounds.left) / bounds.width)),
        y: Math.min(1, Math.max(0, (clientY - bounds.top) / bounds.height))
    };
}
