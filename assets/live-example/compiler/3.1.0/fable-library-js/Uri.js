import { Exception } from "./Util.js";
export const UriKind = {
    RelativeOrAbsolute: 0,
    Absolute: 1,
    Relative: 2,
};
const ok = (value) => ({
    tag: "ok",
    value,
});
const error = (error) => ({ tag: "error", error });
export class Uri {
    constructor(state) {
        this.uri = state;
    }
    static isAbsoluteUri(uri) {
        try {
            new URL(uri);
            return true;
        }
        catch {
            return false;
        }
    }
    static tryCreateWithKind(uri, kind) {
        switch (kind) {
            case UriKind.Absolute:
                return Uri.isAbsoluteUri(uri)
                    ? ok(new Uri({ original: uri, value: new URL(uri), kind }))
                    : error("Invalid URI: The format of the URI could not be determined.");
            case UriKind.Relative:
                return Uri.isAbsoluteUri(uri)
                    ? error("URI is not a relative path.")
                    : ok(new Uri({ original: uri, value: uri, kind }));
            case UriKind.RelativeOrAbsolute:
                return Uri.isAbsoluteUri(uri)
                    ? ok(new Uri({ original: uri, value: new URL(uri), kind: UriKind.Absolute }))
                    : ok(new Uri({ original: uri, value: uri, kind: UriKind.Relative }));
            default:
                const never = kind;
                return never;
        }
    }
    static tryCreateWithBase(baseUri, relativeUri) {
        return baseUri.uri.kind !== UriKind.Absolute
            ? error("Base URI should have Absolute kind")
            : typeof relativeUri === "string"
                ? ok(new Uri({
                    original: new URL(relativeUri, baseUri.uri.value).toString(),
                    value: new URL(relativeUri, baseUri.uri.value),
                    kind: UriKind.Absolute,
                }))
                : relativeUri.uri.kind === UriKind.Relative
                    ? ok(new Uri({
                        original: new URL(relativeUri.uri.value, baseUri.uri.value).toString(),
                        value: new URL(relativeUri.uri.value, baseUri.uri.value),
                        kind: UriKind.Absolute,
                    }))
                    : ok(baseUri);
    }
    static tryCreateImpl(value, kindOrUri = UriKind.Absolute) {
        return typeof value === "string"
            ? typeof kindOrUri !== "number"
                ? error("Kind must be specified when the baseUri is a string.")
                : Uri.tryCreateWithKind(value, kindOrUri)
            : typeof kindOrUri === "number"
                ? error("Kind should not be specified when the baseUri is an absolute Uri.")
                : Uri.tryCreateWithBase(value, kindOrUri);
    }
    static create(value, kindOrUri = UriKind.Absolute) {
        const result = Uri.tryCreateImpl(value, kindOrUri);
        switch (result.tag) {
            case "ok":
                return result.value;
            case "error":
                throw new Exception(result.error);
            default:
                const never = result;
                return never;
        }
    }
    static tryCreate(value, kindOrUri = UriKind.Absolute, out) {
        const result = Uri.tryCreateImpl(value, kindOrUri);
        switch (result.tag) {
            case "ok":
                out.contents = result.value;
                return true;
            case "error":
                return false;
            default:
                const never = result;
                return never;
        }
    }
    toString() {
        switch (this.uri.kind) {
            case UriKind.Absolute:
                return decodeURIComponent(this.asUrl().toString());
            case UriKind.Relative:
                return this.uri.value;
            default:
                const never = this.uri;
                return never;
        }
    }
    asUrl() {
        switch (this.uri.kind) {
            case UriKind.Absolute:
                return this.uri.value;
            case UriKind.Relative:
                throw new Exception("This operation is not supported for a relative URI.");
            default:
                const never = this.uri;
                return never;
        }
    }
    get isAbsoluteUri() {
        return this.uri.kind === UriKind.Absolute;
    }
    get absoluteUri() {
        return this.asUrl().href;
    }
    get scheme() {
        const protocol = this.asUrl().protocol;
        return protocol.slice(0, protocol.length - 1);
    }
    get host() {
        const host = this.asUrl().host;
        if (host.includes(":")) {
            return host.split(":")[0];
        }
        else {
            return host;
        }
    }
    get absolutePath() {
        return this.asUrl().pathname;
    }
    get query() {
        return this.asUrl().search;
    }
    get isDefaultPort() {
        return this.port === 80;
    }
    get port() {
        const port = this.asUrl().port;
        if (port === "") {
            return 80;
        }
        else {
            return parseInt(port);
        }
    }
    get pathAndQuery() {
        const url = this.asUrl();
        return url.pathname + url.search;
    }
    get fragment() {
        return this.asUrl().hash;
    }
    get originalString() {
        return this.uri.original;
    }
}
export default Uri;
