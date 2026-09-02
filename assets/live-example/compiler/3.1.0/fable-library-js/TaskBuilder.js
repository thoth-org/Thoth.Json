import { zero } from "./Task.js";
export class TaskBuilder {
    Bind(computation, binder) {
        return computation.then(binder);
    }
    Combine(computation1, computation2) {
        return computation1.then(computation2);
    }
    Delay(generator) {
        return generator;
    }
    For(sequence, body) {
        const iter = sequence[Symbol.iterator]();
        let cur = iter.next();
        return this.While(() => !cur.done, this.Delay(() => {
            const res = body(cur.value);
            cur = iter.next();
            return res;
        }));
    }
    Return(value) {
        return Promise.resolve(value);
    }
    ReturnFrom(computation) {
        return computation;
    }
    TryFinally(computation, compensation) {
        try {
            return computation().finally(compensation);
        }
        catch (e) {
            compensation();
            return Promise.reject(e);
        }
    }
    TryWith(computation, catchHandler) {
        try {
            return computation().catch(catchHandler);
        }
        catch (e) {
            try {
                return catchHandler(e);
            }
            catch (e2) {
                return Promise.reject(e2);
            }
        }
    }
    Using(resource, binder) {
        return this.TryFinally(() => binder(resource), () => resource.Dispose());
    }
    While(guard, computation) {
        return (async () => {
            while (guard()) {
                await computation();
            }
        })();
    }
    Zero() {
        return zero();
    }
    Run(computation) {
        try {
            return computation();
        }
        catch (e) {
            return Promise.reject(e);
        }
    }
}
export const singleton = new TaskBuilder();
export function task() { return singleton; }
