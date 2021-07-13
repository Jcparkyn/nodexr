Blazor.start({
    loadBootResource: function (type, name, defaultUri, integrity) {
        // For framework resources, use the precompressed .br files for faster downloads
        // This is needed only because GitHub pages doesn't natively support Brotli (or even gzip for .dll files)
        if (type !== 'dotnetjs' && location.hostname !== 'localhost') {
            return (async function () {
                const response = await fetch(defaultUri + '.br', { cache: 'no-cache' });
                if (!response.ok) {
                    throw new Error(response.statusText);
                }
                const originalResponseBuffer = await response.arrayBuffer();
                const originalResponseArray = new Int8Array(originalResponseBuffer);
                const decompressedResponseArray = BrotliDecode(originalResponseArray);
                const contentType = type === 'dotnetwasm' ? 'application/wasm' : 'application/octet-stream';
                return new Response(decompressedResponseArray, { headers: { 'content-type': contentType } });
            })();
        }
    }
});