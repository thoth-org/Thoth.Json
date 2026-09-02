import { OperationCanceledException } from "./AsyncBuilder.js";
export class TaskCompletionSource {
    constructor() {
        this.task = new Promise((resolve, reject) => {
            this._resolve = resolve;
            this._reject = reject;
        });
    }
    SetResult(value) { this._resolve(value); }
    SetException(error) { this._reject(error); }
    SetCancelled() { this._reject(new OperationCanceledException()); }
    get_Task() { return this.task; }
}
export function fromResult(value) {
    return Promise.resolve(value);
}
export function zero() {
    return Promise.resolve();
}
// Task<T> = Promise<T> in JS/TS. GetAwaiter/GetResult return the Promise itself;
// callers should use Async.AwaitTask to extract the value in an async context.
export function getAwaiter(t) {
    return t;
}
export function getResult(t) {
    return t;
}
