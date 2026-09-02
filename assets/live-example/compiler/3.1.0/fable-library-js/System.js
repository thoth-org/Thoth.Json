import { defaultOf, Exception } from "./Util.js";
import { class_type } from "./Reflection.js";
import { SR_Arg_ArgumentOutOfRangeException, SR_ArgumentNull_Generic, SR_Arg_ArgumentException, SR_Arg_ParamName_Name, SR_Arg_TimeoutException, SR_Arg_StackOverflowException, SR_Arg_RankException, SR_Arg_OverflowException, SR_Arg_OutOfMemoryException, SR_Arg_NullReferenceException, SR_Arg_NotSupportedException, SR_Arg_NotImplementedException, SR_Arg_NotFiniteNumberException, SR_Arg_InvalidOperationException, SR_Arg_IndexOutOfRangeException, SR_Arg_FormatException, SR_Arg_DivideByZero, SR_Arg_ArithmeticException, SR_Arg_ApplicationException, SR_Arg_SystemException } from "./Global.js";
import { isNullOrEmpty } from "./String.js";
export class SystemException extends Exception {
    constructor(message) {
        super(message);
    }
}
export function SystemException_$reflection() {
    return class_type("System.SystemException", undefined, SystemException, class_type("System.Exception"));
}
export function SystemException_$ctor_Z721C83C5(message) {
    return new SystemException(message);
}
export function SystemException_$ctor() {
    return SystemException_$ctor_Z721C83C5(SR_Arg_SystemException);
}
export class ApplicationException extends Exception {
    constructor(message) {
        super(message);
    }
}
export function ApplicationException_$reflection() {
    return class_type("System.ApplicationException", undefined, ApplicationException, class_type("System.Exception"));
}
export function ApplicationException_$ctor_Z721C83C5(message) {
    return new ApplicationException(message);
}
export function ApplicationException_$ctor() {
    return ApplicationException_$ctor_Z721C83C5(SR_Arg_ApplicationException);
}
export class ArithmeticException extends Exception {
    constructor(message) {
        super(message);
    }
}
export function ArithmeticException_$reflection() {
    return class_type("System.ArithmeticException", undefined, ArithmeticException, class_type("System.Exception"));
}
export function ArithmeticException_$ctor_Z721C83C5(message) {
    return new ArithmeticException(message);
}
export function ArithmeticException_$ctor() {
    return ArithmeticException_$ctor_Z721C83C5(SR_Arg_ArithmeticException);
}
export class DivideByZeroException extends Exception {
    constructor(message) {
        super(message);
    }
}
export function DivideByZeroException_$reflection() {
    return class_type("System.DivideByZeroException", undefined, DivideByZeroException, class_type("System.Exception"));
}
export function DivideByZeroException_$ctor_Z721C83C5(message) {
    return new DivideByZeroException(message);
}
export function DivideByZeroException_$ctor() {
    return DivideByZeroException_$ctor_Z721C83C5(SR_Arg_DivideByZero);
}
export class FormatException extends Exception {
    constructor(message) {
        super(message);
    }
}
export function FormatException_$reflection() {
    return class_type("System.FormatException", undefined, FormatException, class_type("System.Exception"));
}
export function FormatException_$ctor_Z721C83C5(message) {
    return new FormatException(message);
}
export function FormatException_$ctor() {
    return FormatException_$ctor_Z721C83C5(SR_Arg_FormatException);
}
export class IndexOutOfRangeException extends Exception {
    constructor(message) {
        super(message);
    }
}
export function IndexOutOfRangeException_$reflection() {
    return class_type("System.IndexOutOfRangeException", undefined, IndexOutOfRangeException, class_type("System.Exception"));
}
export function IndexOutOfRangeException_$ctor_Z721C83C5(message) {
    return new IndexOutOfRangeException(message);
}
export function IndexOutOfRangeException_$ctor() {
    return IndexOutOfRangeException_$ctor_Z721C83C5(SR_Arg_IndexOutOfRangeException);
}
export class InvalidOperationException extends Exception {
    constructor(message) {
        super(message);
    }
}
export function InvalidOperationException_$reflection() {
    return class_type("System.InvalidOperationException", undefined, InvalidOperationException, class_type("System.Exception"));
}
export function InvalidOperationException_$ctor_Z721C83C5(message) {
    return new InvalidOperationException(message);
}
export function InvalidOperationException_$ctor() {
    return InvalidOperationException_$ctor_Z721C83C5(SR_Arg_InvalidOperationException);
}
export class NotFiniteNumberException extends Exception {
    constructor(message) {
        super(message);
    }
}
export function NotFiniteNumberException_$reflection() {
    return class_type("System.NotFiniteNumberException", undefined, NotFiniteNumberException, class_type("System.Exception"));
}
export function NotFiniteNumberException_$ctor_Z721C83C5(message) {
    return new NotFiniteNumberException(message);
}
export function NotFiniteNumberException_$ctor() {
    return NotFiniteNumberException_$ctor_Z721C83C5(SR_Arg_NotFiniteNumberException);
}
export class NotImplementedException extends Exception {
    constructor(message) {
        super(message);
    }
}
export function NotImplementedException_$reflection() {
    return class_type("System.NotImplementedException", undefined, NotImplementedException, class_type("System.Exception"));
}
export function NotImplementedException_$ctor_Z721C83C5(message) {
    return new NotImplementedException(message);
}
export function NotImplementedException_$ctor() {
    return NotImplementedException_$ctor_Z721C83C5(SR_Arg_NotImplementedException);
}
export class NotSupportedException extends Exception {
    constructor(message) {
        super(message);
    }
}
export function NotSupportedException_$reflection() {
    return class_type("System.NotSupportedException", undefined, NotSupportedException, class_type("System.Exception"));
}
export function NotSupportedException_$ctor_Z721C83C5(message) {
    return new NotSupportedException(message);
}
export function NotSupportedException_$ctor() {
    return NotSupportedException_$ctor_Z721C83C5(SR_Arg_NotSupportedException);
}
export class NullReferenceException extends Exception {
    constructor(message) {
        super(message);
    }
}
export function NullReferenceException_$reflection() {
    return class_type("System.NullReferenceException", undefined, NullReferenceException, class_type("System.Exception"));
}
export function NullReferenceException_$ctor_Z721C83C5(message) {
    return new NullReferenceException(message);
}
export function NullReferenceException_$ctor() {
    return NullReferenceException_$ctor_Z721C83C5(SR_Arg_NullReferenceException);
}
export class OutOfMemoryException extends Exception {
    constructor(message) {
        super(message);
    }
}
export function OutOfMemoryException_$reflection() {
    return class_type("System.OutOfMemoryException", undefined, OutOfMemoryException, class_type("System.Exception"));
}
export function OutOfMemoryException_$ctor_Z721C83C5(message) {
    return new OutOfMemoryException(message);
}
export function OutOfMemoryException_$ctor() {
    return OutOfMemoryException_$ctor_Z721C83C5(SR_Arg_OutOfMemoryException);
}
export class OverflowException extends Exception {
    constructor(message) {
        super(message);
    }
}
export function OverflowException_$reflection() {
    return class_type("System.OverflowException", undefined, OverflowException, class_type("System.Exception"));
}
export function OverflowException_$ctor_Z721C83C5(message) {
    return new OverflowException(message);
}
export function OverflowException_$ctor() {
    return OverflowException_$ctor_Z721C83C5(SR_Arg_OverflowException);
}
export class RankException extends Exception {
    constructor(message) {
        super(message);
    }
}
export function RankException_$reflection() {
    return class_type("System.RankException", undefined, RankException, class_type("System.Exception"));
}
export function RankException_$ctor_Z721C83C5(message) {
    return new RankException(message);
}
export function RankException_$ctor() {
    return RankException_$ctor_Z721C83C5(SR_Arg_RankException);
}
export class StackOverflowException extends Exception {
    constructor(message) {
        super(message);
    }
}
export function StackOverflowException_$reflection() {
    return class_type("System.StackOverflowException", undefined, StackOverflowException, class_type("System.Exception"));
}
export function StackOverflowException_$ctor_Z721C83C5(message) {
    return new StackOverflowException(message);
}
export function StackOverflowException_$ctor() {
    return StackOverflowException_$ctor_Z721C83C5(SR_Arg_StackOverflowException);
}
export class TimeoutException extends Exception {
    constructor(message) {
        super(message);
    }
}
export function TimeoutException_$reflection() {
    return class_type("System.TimeoutException", undefined, TimeoutException, class_type("System.Exception"));
}
export function TimeoutException_$ctor_Z721C83C5(message) {
    return new TimeoutException(message);
}
export function TimeoutException_$ctor() {
    return TimeoutException_$ctor_Z721C83C5(SR_Arg_TimeoutException);
}
export class ArgumentException extends Exception {
    constructor(message, paramName, innerException) {
        super(isNullOrEmpty(paramName) ? message : (((message + SR_Arg_ParamName_Name) + paramName) + "\')"), innerException);
        this.paramName = paramName;
    }
}
export function ArgumentException_$reflection() {
    return class_type("System.ArgumentException", undefined, ArgumentException, class_type("System.Exception"));
}
export function ArgumentException_$ctor_Z60A2B367(message, paramName, innerException) {
    return new ArgumentException(message, paramName, innerException);
}
export function ArgumentException_$ctor() {
    return ArgumentException_$ctor_Z60A2B367(SR_Arg_ArgumentException, "", defaultOf());
}
export function ArgumentException_$ctor_Z721C83C5(message) {
    return ArgumentException_$ctor_Z60A2B367(message, "", defaultOf());
}
export function ArgumentException_$ctor_Z384F8060(message, paramName) {
    return ArgumentException_$ctor_Z60A2B367(message, paramName, defaultOf());
}
export function ArgumentException_$ctor_68CE3CA2(message, innerException) {
    return ArgumentException_$ctor_Z60A2B367(message, "", innerException);
}
export function ArgumentException__get_ParamName(_) {
    return _.paramName;
}
export class ArgumentNullException extends ArgumentException {
    constructor(paramName, message) {
        super(message, paramName, defaultOf());
    }
}
export function ArgumentNullException_$reflection() {
    return class_type("System.ArgumentNullException", undefined, ArgumentNullException, ArgumentException_$reflection());
}
export function ArgumentNullException_$ctor_Z384F8060(paramName, message) {
    return new ArgumentNullException(paramName, message);
}
export function ArgumentNullException_$ctor_Z721C83C5(paramName) {
    return ArgumentNullException_$ctor_Z384F8060(paramName, SR_ArgumentNull_Generic);
}
export function ArgumentNullException_$ctor() {
    return ArgumentNullException_$ctor_Z721C83C5("");
}
export class ArgumentOutOfRangeException extends ArgumentException {
    constructor(paramName, message) {
        super(message, paramName, defaultOf());
    }
}
export function ArgumentOutOfRangeException_$reflection() {
    return class_type("System.ArgumentOutOfRangeException", undefined, ArgumentOutOfRangeException, ArgumentException_$reflection());
}
export function ArgumentOutOfRangeException_$ctor_Z384F8060(paramName, message) {
    return new ArgumentOutOfRangeException(paramName, message);
}
export function ArgumentOutOfRangeException_$ctor_Z721C83C5(paramName) {
    return ArgumentOutOfRangeException_$ctor_Z384F8060(paramName, SR_Arg_ArgumentOutOfRangeException);
}
export function ArgumentOutOfRangeException_$ctor() {
    return ArgumentOutOfRangeException_$ctor_Z721C83C5("");
}
