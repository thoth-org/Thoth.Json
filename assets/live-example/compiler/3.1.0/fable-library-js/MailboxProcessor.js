import { defaultCancellationToken } from "./Async.js";
import { fromContinuations } from "./Async.js";
import { startImmediate } from "./Async.js";
import { Exception } from "./Util.js";
class QueueCell {
    constructor(message) {
        this.value = message;
    }
}
class MailboxQueue {
    add(message) {
        const itCell = new QueueCell(message);
        if (this.firstAndLast) {
            this.firstAndLast[1].next = itCell;
            this.firstAndLast = [this.firstAndLast[0], itCell];
        }
        else {
            this.firstAndLast = [itCell, itCell];
        }
    }
    tryGet() {
        if (this.firstAndLast) {
            const value = this.firstAndLast[0].value;
            if (this.firstAndLast[0].next) {
                this.firstAndLast = [this.firstAndLast[0].next, this.firstAndLast[1]];
            }
            else {
                delete this.firstAndLast;
            }
            // Wrap the value so falsy messages (0, false, "", null, undefined/unit)
            // can be distinguished from an empty queue.
            return { value };
        }
        return void 0;
    }
}
export class MailboxProcessor {
    constructor(body, cancellationToken) {
        this.body = body;
        this.cancellationToken = cancellationToken || defaultCancellationToken;
        this.messages = new MailboxQueue();
    }
}
function __processEvents($this) {
    if ($this.continuation) {
        const dequeued = $this.messages.tryGet();
        if (dequeued !== void 0) {
            const cont = $this.continuation;
            delete $this.continuation;
            cont(dequeued.value);
        }
    }
}
export function startInstance($this) {
    startImmediate($this.body($this), $this.cancellationToken);
}
export function receive($this) {
    return fromContinuations((conts) => {
        if ($this.continuation) {
            throw new Exception("Receive can only be called once!");
        }
        $this.continuation = conts[0];
        __processEvents($this);
    });
}
export function post($this, message) {
    $this.messages.add(message);
    __processEvents($this);
}
export function postAndAsyncReply($this, buildMessage) {
    let result;
    // Use an explicit flag because the reply value itself may be undefined
    // (e.g. AsyncReplyChannel<unit>), which must still complete the async.
    let replied = false;
    let continuation;
    function checkCompletion() {
        if (replied && continuation !== void 0) {
            continuation(result);
        }
    }
    const reply = {
        reply: (res) => {
            result = res;
            replied = true;
            checkCompletion();
        },
    };
    $this.messages.add(buildMessage(reply));
    __processEvents($this);
    return fromContinuations((conts) => {
        continuation = conts[0];
        checkCompletion();
    });
}
export function start(body, cancellationToken) {
    const mbox = new MailboxProcessor(body, cancellationToken);
    startInstance(mbox);
    return mbox;
}
export default MailboxProcessor;
