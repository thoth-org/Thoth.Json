import { OperationCanceledException, Trampoline } from "./AsyncBuilder.js";
import { CancellationToken } from "./AsyncBuilder.js";
import { protectedCont, protectedBind, protectedReturn } from "./AsyncBuilder.js";
import { Choice_makeChoice1Of2, Choice_makeChoice2Of2 } from "./Choice.js";
import { TimeoutException_$ctor } from "./System.js";
import { Exception } from "./Util.js";
function emptyContinuation(_x) {
    // NOP
}
// MakeAsync: body:(AsyncActivation<'T> -> AsyncReturn) -> Async<'T>
export function makeAsync(body) {
    return body;
}
// Invoke: computation: Async<'T> -> ctxt:AsyncActivation<'T> -> AsyncReturn
export function invoke(computation, ctx) {
    return computation(ctx);
}
// CallThenInvoke: ctxt:AsyncActivation<'T> -> result1:'U -> part2:('U -> Async<'T>) -> AsyncReturn
export function callThenInvoke(ctx, result1, part2) {
    return part2(result1)(ctx);
}
// Bind: ctxt:AsyncActivation<'T> -> part1:Async<'U> -> part2:('U -> Async<'T>) -> AsyncReturn
export function bind(ctx, part1, part2) {
    return protectedBind(part1, part2)(ctx);
}
export function createCancellationToken(arg) {
    const token = new CancellationToken(typeof arg === "boolean" ? arg : false);
    if (typeof arg === "number") {
        setTimeout(() => { token.cancel(); }, arg);
    }
    return token;
}
export function cancel(token) {
    token.cancel();
}
export function cancelAfter(token, ms) {
    setTimeout(() => { token.cancel(); }, ms);
}
export function isCancellationRequested(token) {
    return token != null && token.isCancelled;
}
export function throwIfCancellationRequested(token) {
    if (token != null && token.isCancelled) {
        throw new Exception("Operation is cancelled");
    }
}
export function startChild(computation, ms) {
    return protectedCont((ctx) => {
        // Share the parent's cancellation token so cancelling the parent
        // cancels the child, like .NET Async.StartChild
        const promise = startAsPromise(computation, ctx.cancelToken);
        let promiseToRun = promise;
        if (ms) {
            // Race the computation against a timeout: whichever settles first wins.
            promiseToRun = new Promise((resolve, reject) => {
                const timeoutId = setTimeout(() => reject(TimeoutException_$ctor()), ms);
                promise.then(value => { clearTimeout(timeoutId); resolve(value); }, error => { clearTimeout(timeoutId); reject(error); });
            });
        }
        // Prevent an unhandled rejection when the child is cancelled or fails
        // while nobody awaits it (in F# an unobserved child's cancellation or
        // failure is simply not propagated). awaitPromise attaches its own
        // handlers to the same promise, so awaiting still observes the result.
        promiseToRun.catch(emptyContinuation);
        // JS Promises are hot, computation has already started
        // but we delay returning the result
        protectedReturn(awaitPromise(promiseToRun))(ctx);
    });
}
export function awaitPromise(p) {
    return fromContinuations((conts) => p.then(conts[0]).catch((err) => (err instanceof OperationCanceledException
        ? conts[2] : conts[1])(err)));
}
export function awaitEvent(event, cancelAction) {
    return protectedCont((ctx) => {
        let tokenId;
        const handler = ((_sender, arg) => {
            ctx.cancelToken.removeListener(tokenId);
            event.RemoveHandler(handler);
            ctx.onSuccess(arg);
        });
        tokenId = ctx.cancelToken.addListener(() => {
            event.RemoveHandler(handler);
            if (cancelAction != null) {
                cancelAction();
            }
            ctx.onCancel(new OperationCanceledException());
        });
        event.AddHandler(handler);
    });
}
export function cancellationToken() {
    return protectedCont((ctx) => ctx.onSuccess(ctx.cancelToken));
}
export const defaultCancellationToken = new CancellationToken();
export function catchAsync(work) {
    return protectedCont((ctx) => {
        work({
            onSuccess: (x) => ctx.onSuccess(Choice_makeChoice1Of2(x)),
            onError: (ex) => ctx.onSuccess(Choice_makeChoice2Of2(ex)),
            onCancel: ctx.onCancel,
            cancelToken: ctx.cancelToken,
            trampoline: ctx.trampoline,
        });
    });
}
export function fromContinuations(f) {
    return protectedCont((ctx) => f([ctx.onSuccess, ctx.onError, ctx.onCancel]));
}
export function ignore(computation) {
    return protectedBind(computation, (_x) => protectedReturn(void 0));
}
export function parallel(computations) {
    // Children share the parent's cancellation token, like .NET Async.Parallel
    return protectedCont((ctx) => awaitPromise(Promise.all(Array.from(computations, (w) => startAsPromise(w, ctx.cancelToken))))(ctx));
}
export function sequential(computations) {
    function _sequential(computations, cancelToken) {
        let pr = Promise.resolve([]);
        for (const c of computations) {
            pr = pr.then(results => startAsPromise(c, cancelToken).then(r => results.concat([r])));
        }
        return pr;
    }
    // Children share the parent's cancellation token, like .NET Async.Sequential
    return protectedCont((ctx) => awaitPromise(_sequential(computations, ctx.cancelToken))(ctx));
}
export function sleep(millisecondsDueTime) {
    return protectedCont((ctx) => {
        let tokenId;
        const timeoutId = setTimeout(() => {
            ctx.cancelToken.removeListener(tokenId);
            ctx.onSuccess(void 0);
        }, millisecondsDueTime);
        tokenId = ctx.cancelToken.addListener(() => {
            clearTimeout(timeoutId);
            ctx.onCancel(new OperationCanceledException());
        });
    });
}
export function start(computation, cancellationToken) {
    return startWithContinuations(computation, emptyContinuation, function (err) { throw err; }, emptyContinuation, cancellationToken);
}
export function startImmediate(computation, cancellationToken) {
    return start(computation, cancellationToken);
}
export function startWithContinuations(computation, continuation, exceptionContinuation, cancellationContinuation, cancelToken) {
    const trampoline = new Trampoline();
    // Mark the computation completed as soon as a terminal continuation is entered
    // so protectedCont lets exceptions from continuation code propagate instead of
    // routing them to onError (see Trampoline.completed).
    const done = (cont) => (x) => { trampoline.completed = true; return cont(x); };
    computation({
        onSuccess: done(continuation ? continuation : emptyContinuation),
        onError: done(exceptionContinuation),
        onCancel: done(cancellationContinuation),
        cancelToken: cancelToken ? cancelToken : defaultCancellationToken,
        trampoline,
    });
}
export function startAsPromise(computation, cancellationToken) {
    return new Promise((resolve, reject) => startWithContinuations(computation, resolve, reject, reject, cancellationToken ? cancellationToken : defaultCancellationToken));
}
