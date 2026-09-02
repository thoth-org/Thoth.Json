/**
 * DateTimeOffset functions.
 *
 * Note: Date instances are always DateObjects in local
 * timezone (because JS dates are all kinds of messed up).
 * A local date returns UTC epoch when `.getTime()` is called.
 *
 * Basically; invariant: date.getTime() always return UTC time.
 */
import { toInt64, toFloat64 } from "./BigInt.js";
import { Exception, compareDates, DateTimeKind, dateOffset, padWithZeros } from "./Util.js";
const shortDays = [
    "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"
];
const longDays = [
    "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
];
const shortMonths = [
    "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
];
const longMonths = [
    "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"
];
function parseRepeatToken(format, pos, patternChar) {
    let tokenLength = 0;
    let internalPos = pos;
    while (internalPos < format.length && format[internalPos] === patternChar) {
        internalPos++;
        tokenLength++;
    }
    return tokenLength;
}
function parseNextChar(format, pos) {
    if (pos >= format.length - 1) {
        return -1;
    }
    return format.charCodeAt(pos + 1);
}
function parseQuotedString(format, pos) {
    let beginPos = pos;
    // Get the character used to quote the string
    const quoteChar = format[pos];
    let result = "";
    let foundQuote = false;
    while (pos < format.length) {
        pos++;
        const currentChar = format[pos];
        if (currentChar === quoteChar) {
            foundQuote = true;
            break;
        }
        else if (currentChar === "\\") {
            if (pos < format.length) {
                pos++;
                result += format[pos];
            }
            else {
                // This means that '\' is the last character in the string.
                throw new Exception("Invalid string format");
            }
        }
        else {
            result += currentChar;
        }
    }
    if (!foundQuote) {
        // We could not find the matching quote
        throw new Exception(`Invalid string format could not find matching quote for ${quoteChar}`);
    }
    return [result, pos - beginPos + 1];
}
function dateToStringWithCustomFormat(date, format, utc) {
    let cursorPos = 0;
    let tokenLength = 0;
    let result = "";
    const localizedDate = utc ? DateTime(date.getTime(), DateTimeKind.Utc) : date;
    while (cursorPos < format.length) {
        const token = format[cursorPos];
        switch (token) {
            case "d":
                tokenLength = parseRepeatToken(format, cursorPos, "d");
                cursorPos += tokenLength;
                switch (tokenLength) {
                    case 1:
                        result += day(localizedDate);
                        break;
                    case 2:
                        result += padWithZeros(day(localizedDate), 2);
                        break;
                    case 3:
                        result += shortDays[dayOfWeek(localizedDate)];
                        break;
                    case 4:
                    default:
                        result += longDays[dayOfWeek(localizedDate)];
                        break;
                }
                break;
            case "f":
                tokenLength = parseRepeatToken(format, cursorPos, "f");
                cursorPos += tokenLength;
                if (tokenLength <= 3) {
                    const precision = 10 ** (3 - tokenLength);
                    result += padWithZeros(Math.floor(millisecond(localizedDate) / precision), tokenLength);
                }
                else if (tokenLength <= 7) {
                    // JavaScript Date only support precision to the millisecond
                    // so we fill the rest of the precision with 0 as if the date didn't have
                    // milliseconds provided to it.
                    // This is to have the same behavior as .NET when doing:
                    // DateTime(1, 2, 3, 4, 5, 6, DateTimeKind.Utc).ToString("fffff") => 00000
                    result += ("" + millisecond(localizedDate)).padEnd(tokenLength, "0");
                }
                else {
                    throw "Input string was not in a correct format.";
                }
                break;
            case "F":
                tokenLength = parseRepeatToken(format, cursorPos, "F");
                cursorPos += tokenLength;
                if (tokenLength <= 3) {
                    const precision = 10 ** (3 - tokenLength);
                    const value = Math.floor(millisecond(localizedDate) / precision);
                    if (value != 0) {
                        result += padWithZeros(value, tokenLength);
                    }
                }
                else if (tokenLength <= 7) {
                    // JavaScript Date only support precision to the millisecond
                    // so we can't go beyond that.
                    // We also need to pad start with 0 if the value is not 0
                    const value = millisecond(localizedDate);
                    if (value != 0) {
                        result += padWithZeros(value, 3);
                    }
                }
                else {
                    throw "Input string was not in a correct format.";
                }
                break;
            case "g":
                tokenLength = parseRepeatToken(format, cursorPos, "g");
                cursorPos += tokenLength;
                result += "A.D.";
                break;
            case "h":
                tokenLength = parseRepeatToken(format, cursorPos, "h");
                cursorPos += tokenLength;
                switch (tokenLength) {
                    case 1:
                        const h1Value = hour(localizedDate) % 12;
                        result += h1Value ? h1Value : 12;
                        break;
                    case 2:
                    default:
                        const h2Value = hour(localizedDate) % 12;
                        result += padWithZeros(h2Value ? h2Value : 12, 2);
                        break;
                }
                break;
            case "H":
                tokenLength = parseRepeatToken(format, cursorPos, "H");
                cursorPos += tokenLength;
                switch (tokenLength) {
                    case 1:
                        result += hour(localizedDate);
                        break;
                    case 2:
                    default:
                        result += padWithZeros(hour(localizedDate), 2);
                        break;
                }
                break;
            case "K":
                tokenLength = parseRepeatToken(format, cursorPos, "K");
                cursorPos += tokenLength;
                switch (tokenLength) {
                    case 1:
                        switch (getKind(localizedDate)) {
                            case DateTimeKind.Utc:
                                result += "Z";
                                break;
                            case DateTimeKind.Local:
                                result += dateOffsetToString(localizedDate.getTimezoneOffset() * -60000);
                                break;
                            case DateTimeKind.Unspecified:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                break;
            case "m":
                tokenLength = parseRepeatToken(format, cursorPos, "m");
                cursorPos += tokenLength;
                switch (tokenLength) {
                    case 1:
                        result += minute(localizedDate);
                        break;
                    case 2:
                    default:
                        result += padWithZeros(minute(localizedDate), 2);
                        break;
                }
                break;
            case "M":
                tokenLength = parseRepeatToken(format, cursorPos, "M");
                cursorPos += tokenLength;
                switch (tokenLength) {
                    case 1:
                        result += month(localizedDate);
                        break;
                    case 2:
                        result += padWithZeros(month(localizedDate), 2);
                        break;
                    case 3:
                        result += shortMonths[month(localizedDate) - 1];
                        break;
                    case 4:
                    default:
                        result += longMonths[month(localizedDate) - 1];
                        break;
                }
                break;
            case "s":
                tokenLength = parseRepeatToken(format, cursorPos, "s");
                cursorPos += tokenLength;
                switch (tokenLength) {
                    case 1:
                        result += second(localizedDate);
                        break;
                    case 2:
                    default:
                        result += padWithZeros(second(localizedDate), 2);
                        break;
                }
                break;
            case "t":
                tokenLength = parseRepeatToken(format, cursorPos, "t");
                cursorPos += tokenLength;
                switch (tokenLength) {
                    case 1:
                        result += localizedDate.getHours() < 12 ? "A" : "P";
                        break;
                    case 2:
                    default:
                        result += localizedDate.getHours() < 12 ? "AM" : "PM";
                        break;
                }
                break;
            case "y":
                tokenLength = parseRepeatToken(format, cursorPos, "y");
                cursorPos += tokenLength;
                switch (tokenLength) {
                    case 1:
                        result += year(localizedDate) % 100;
                        break;
                    case 2:
                        result += padWithZeros(year(localizedDate) % 100, 2);
                        break;
                    default:
                        result += padWithZeros(year(localizedDate), tokenLength);
                        break;
                }
                break;
            case "z":
                tokenLength = parseRepeatToken(format, cursorPos, "z");
                cursorPos += tokenLength;
                let utcOffsetText = "";
                switch (getKind(localizedDate)) {
                    case DateTimeKind.Utc:
                        utcOffsetText = "+00:00";
                        break;
                    case DateTimeKind.Local:
                        utcOffsetText = dateOffsetToString(localizedDate.getTimezoneOffset() * -60000);
                        break;
                    case DateTimeKind.Unspecified:
                        utcOffsetText = dateOffsetToString(toLocalTime(localizedDate).getTimezoneOffset() * -60000);
                        break;
                }
                const sign = utcOffsetText[0] === "-" ? "-" : "+";
                const hours = parseInt(utcOffsetText.substring(1, 3), 10);
                const minutes = parseInt(utcOffsetText.substring(4, 6), 10);
                switch (tokenLength) {
                    case 1:
                        result += `${sign}${hours}`;
                        break;
                    case 2:
                        result += `${sign}${padWithZeros(hours, 2)}`;
                        break;
                    default:
                        result += `${sign}${padWithZeros(hours, 2)}:${padWithZeros(minutes, 2)}`;
                        break;
                }
                break;
            case ":":
                result += ":";
                cursorPos++;
                break;
            case "/":
                result += "/";
                cursorPos++;
                break;
            case "'":
            case '"':
                const [quotedString, quotedStringLenght] = parseQuotedString(format, cursorPos);
                result += quotedString;
                cursorPos += quotedStringLenght;
                break;
            case "%":
                const nextChar = parseNextChar(format, cursorPos);
                if (nextChar >= 0 && nextChar !== "%".charCodeAt(0)) {
                    cursorPos += 2;
                    result += dateToStringWithCustomFormat(localizedDate, String.fromCharCode(nextChar), utc);
                }
                else {
                    throw new Exception("Invalid format string");
                }
                break;
            case "\\":
                const nextChar2 = parseNextChar(format, cursorPos);
                if (nextChar2 >= 0) {
                    cursorPos += 2;
                    result += String.fromCharCode(nextChar2);
                }
                else {
                    throw new Exception("Invalid format string");
                }
                break;
            default:
                cursorPos++;
                result += token;
                break;
        }
    }
    return result;
}
export function getKind(value) {
    return value.kind ?? DateTimeKind.Unspecified;
}
export function unixEpochMillisecondsToTicks(ms, offset) {
    return toInt64(((BigInt(ms) + 62135596800000n) + BigInt(offset)) * 10000n);
}
export function ticksToUnixEpochMilliseconds(ticks) {
    return Number(((BigInt(ticks) / 10000n) - 62135596800000n));
}
export function dateOffsetToString(offset) {
    const isMinus = offset < 0;
    offset = Math.abs(offset);
    const hours = ~~(offset / 3600000);
    const minutes = (offset % 3600000) / 60000;
    return (isMinus ? "-" : "+") +
        padWithZeros(hours, 2) + ":" +
        padWithZeros(minutes, 2);
}
function dateToISOString(d, utc) {
    if (utc) {
        return d.toISOString();
    }
    else {
        // JS Date is always local
        const printOffset = d.kind == null ? true : d.kind === DateTimeKind.Local;
        return padWithZeros(d.getFullYear(), 4) + "-" +
            padWithZeros(d.getMonth() + 1, 2) + "-" +
            padWithZeros(d.getDate(), 2) + "T" +
            padWithZeros(d.getHours(), 2) + ":" +
            padWithZeros(d.getMinutes(), 2) + ":" +
            padWithZeros(d.getSeconds(), 2) + "." +
            padWithZeros(d.getMilliseconds(), 3) +
            (printOffset ? dateOffsetToString(d.getTimezoneOffset() * -60000) : "");
    }
}
function dateToISOStringWithOffset(dateWithOffset, offset) {
    const str = dateWithOffset.toISOString();
    return str.substring(0, str.length - 1) + dateOffsetToString(offset);
}
function dateToStringWithOffset(date, format) {
    const d = new Date(date.getTime() + (date.offset ?? 0));
    if (typeof format !== "string") {
        return d.toISOString().replace(/\.\d+/, "").replace(/[A-Z]|\.\d+/g, " ") + dateOffsetToString((date.offset ?? 0));
    }
    else if (format.length === 1) {
        switch (format) {
            case "D": return dateToString_D(d);
            case "d": return dateToString_d(d);
            case "F": return dateToString_D(d) + " " + dateToString_T(d);
            case "f": return dateToString_D(d) + " " + dateToString_t(d);
            case "G": return dateToString_d(d) + " " + dateToString_T(d);
            case "g": return dateToString_d(d) + " " + dateToString_t(d);
            case "M":
            case "m": return dateToString_M(d);
            case "O":
            case "o": return dateToISOStringWithOffset(d, (date.offset ?? 0));
            case "R":
            case "r": {
                const utcDate = DateTime(date.getTime(), DateTimeKind.Utc);
                return dateToString_R(utcDate);
            }
            case "s": return dateToString_s(toUniversalTime(d));
            case "T": return dateToString_T(toUniversalTime(d));
            case "t": return dateToString_t(toUniversalTime(d));
            case "u": {
                const utcDate = DateTime(date.getTime(), DateTimeKind.Utc);
                return dateToString_u(utcDate);
            }
            case "U": {
                const utcDate = DateTime(date.getTime(), DateTimeKind.Utc);
                return dateToString_D(utcDate) + " " + dateToString_T(utcDate);
            }
            case "Y":
            case "y": return dateToString_Y(d);
            default: throw new Exception("Unrecognized Date print format");
        }
    }
    else {
        return dateToStringWithCustomFormat(d, format, true);
    }
}
function dateToString_D(date) {
    return longDays[dayOfWeek(date)]
        + ", " + padWithZeros(day(date), 2)
        + " " + longMonths[month(date) - 1]
        + " " + year(date);
}
function dateToString_d(date) {
    return padWithZeros(month(date), 2)
        + "/" + padWithZeros(day(date), 2)
        + "/" + year(date);
}
function dateToString_T(date) {
    return padWithZeros(hour(date), 2)
        + ":" + padWithZeros(minute(date), 2)
        + ":" + padWithZeros(second(date), 2);
}
function dateToString_t(date) {
    return padWithZeros(hour(date), 2)
        + ":" + padWithZeros(minute(date), 2);
}
// RFC 1123: "Thu, 01 Jan 2009 00:00:00 GMT" — always UTC
function dateToString_R(date) {
    const utcDate = toUniversalTime(date);
    return shortDays[dayOfWeek(utcDate)] + ", "
        + padWithZeros(day(utcDate), 2) + " "
        + shortMonths[month(utcDate) - 1] + " "
        + year(utcDate) + " "
        + padWithZeros(hour(utcDate), 2) + ":"
        + padWithZeros(minute(utcDate), 2) + ":"
        + padWithZeros(second(utcDate), 2) + " GMT";
}
// Sortable ISO 8601, no timezone: "2009-06-15T13:45:30"
function dateToString_s(date) {
    return padWithZeros(year(date), 4) + "-"
        + padWithZeros(month(date), 2) + "-"
        + padWithZeros(day(date), 2) + "T"
        + padWithZeros(hour(date), 2) + ":"
        + padWithZeros(minute(date), 2) + ":"
        + padWithZeros(second(date), 2);
}
// Universal sortable: "2009-06-15 13:45:30Z" — always UTC
function dateToString_u(date) {
    const utcDate = toUniversalTime(date);
    return padWithZeros(year(utcDate), 4) + "-"
        + padWithZeros(month(utcDate), 2) + "-"
        + padWithZeros(day(utcDate), 2) + " "
        + padWithZeros(hour(utcDate), 2) + ":"
        + padWithZeros(minute(utcDate), 2) + ":"
        + padWithZeros(second(utcDate), 2) + "Z";
}
// Month/day (InvariantCulture "MMMM dd"): "June 15"
function dateToString_M(date) {
    return longMonths[month(date) - 1] + " " + padWithZeros(day(date), 2);
}
// Year/month (InvariantCulture "yyyy MMMM"): "2009 June"
function dateToString_Y(date) {
    return year(date) + " " + longMonths[month(date) - 1];
}
function dateToStringWithKind(date, format) {
    const utc = date.kind === DateTimeKind.Utc;
    if (typeof format !== "string") {
        return dateToString_d(date) + " " + dateToString_T(date);
    }
    else if (format.length === 1) {
        switch (format) {
            case "D": return dateToString_D(date);
            case "d": return dateToString_d(date);
            case "F": return dateToString_D(date) + " " + dateToString_T(date);
            case "f": return dateToString_D(date) + " " + dateToString_t(date);
            case "G": return dateToString_d(date) + " " + dateToString_T(date);
            case "g": return dateToString_d(date) + " " + dateToString_t(date);
            case "M":
            case "m": return dateToString_M(date);
            case "O":
            case "o": return dateToISOString(date, utc);
            case "R":
            case "r": return dateToString_R(date);
            case "s": return dateToString_s(date);
            case "T": return dateToString_T(date);
            case "t": return dateToString_t(date);
            case "u": return dateToString_u(date);
            case "U": return dateToString_D(toUniversalTime(date)) + " " + dateToString_T(toUniversalTime(date));
            case "Y":
            case "y": return dateToString_Y(date);
            default:
                throw new Exception("Unrecognized Date print format");
        }
    }
    else {
        return dateToStringWithCustomFormat(date, format, utc);
    }
}
export function toString(date, format, _provider) {
    return date.offset != null
        ? dateToStringWithOffset(date, format)
        : dateToStringWithKind(date, format);
}
export function DateTime(value, kind) {
    const d = new Date(value);
    d.kind = (kind == null ? DateTimeKind.Unspecified : kind);
    return d;
}
export function fromTicks(ticks, kind) {
    kind = kind != null ? kind : DateTimeKind.Local; // better default than Unspecified
    let date = DateTime(ticksToUnixEpochMilliseconds(ticks), kind);
    // Ticks are local to offset (in this case, either UTC or Local/Unknown).
    // If kind is anything but UTC, that means that the tick number was not
    // in utc, thus getTime() cannot return UTC, and needs to be shifted.
    if (kind !== DateTimeKind.Utc) {
        date = DateTime(date.getTime() - dateOffset(date), kind);
    }
    return date;
}
export function fromDateTime(dateOnly, timeOnly, kind) {
    const d = DateTime(dateOnly.getTime() + timeOnly, kind);
    return DateTime(d.getTime() - dateOffset(d), kind);
}
export function fromDateTimeOffset(date, kind) {
    switch (kind) {
        case DateTimeKind.Utc: return DateTime(date.getTime(), DateTimeKind.Utc);
        case DateTimeKind.Local: return DateTime(date.getTime(), DateTimeKind.Local);
        default:
            const d = DateTime(date.getTime() + (date.offset ?? 0), kind);
            return DateTime(d.getTime() - dateOffset(d), kind);
    }
}
export function getTicks(date) {
    return unixEpochMillisecondsToTicks(date.getTime(), dateOffset(date));
}
export function minValue() {
    // This is "0001-01-01T00:00:00.000Z", actual JS min value is -8640000000000000
    return DateTime(-62135596800000, DateTimeKind.Utc);
}
export function maxValue() {
    // This is "9999-12-31T23:59:59.999Z", actual JS max value is 8640000000000000
    return DateTime(253402300799999, DateTimeKind.Utc);
}
// The only date words .NET's invariant parser recognises: month names, weekday names,
// meridiem designators and zone markers (plus the ISO "T" separator). Anything else is
// rejected. Used to reject JS-permissive inputs (see `parseRaw`).
const recognizedDateWords = new Set([
    "january", "february", "march", "april", "may", "june",
    "july", "august", "september", "october", "november", "december",
    "jan", "feb", "mar", "apr", "jun", "jul", "aug", "sep", "sept", "oct", "nov", "dec",
    "monday", "tuesday", "wednesday", "thursday", "friday", "saturday", "sunday",
    "mon", "tue", "wed", "thu", "fri", "sat", "sun",
    "am", "pm", "gmt", "utc", "ut", "t", "z",
]);
export function parseRaw(input) {
    function fail() {
        throw new Exception(`The string is not a valid Date: ${input}`);
    }
    if (input == null || input.trim() === "") {
        fail();
    }
    if ((input.match(/[a-z]+/gi) ?? []).some(word => !recognizedDateWords.has(word.toLowerCase()))) {
        fail();
    }
    // ISO dates without TZ are parsed as UTC. Adding time without TZ keeps them local.
    if (input.length === 10 && input[4] === "-" && input[7] === "-") {
        input += "T00:00:00";
    }
    let date = new Date(input);
    let offset = null;
    if (isNaN(date.getTime())) {
        // Try to check strings JS Date cannot parse (see #1045, #1422)
        const m = /^\s*(\d+[^\w\s:]\d+[^\w\s:]\d+)?\s*(\d+:\d+(?::\d+(?:\.\d+)?)?)?\s*([AaPp][Mm])?\s*(Z|[+-]([01]?\d):?([0-5]?\d)?)?\s*$/.exec(input);
        if (m != null) {
            let baseDate;
            let timeInSeconds = 0;
            if (m[2] != null) {
                const timeParts = m[2].split(":");
                const hourPart = parseInt(timeParts[0], 10);
                timeInSeconds =
                    hourPart * 3600 +
                        parseInt(timeParts[1] || "0", 10) * 60 +
                        parseFloat(timeParts[2] || "0");
                if (m[3] != null && m[3].toUpperCase() === "PM" && hourPart < 12) {
                    timeInSeconds += 12 * 3600;
                }
                else if (m[3] != null && m[3].toUpperCase() === "AM" && hourPart === 12) {
                    timeInSeconds -= 12 * 3600;
                }
            }
            if (m[4] != null) { // There's an offset, parse as UTC
                if (m[1] != null) {
                    baseDate = new Date(m[1] + " UTC");
                }
                else {
                    const d = new Date();
                    baseDate = new Date(d.getUTCFullYear() + "/" + (d.getUTCMonth() + 1) + "/" + d.getUTCDate());
                }
                if (m[4] === "Z") {
                    offset = "Z";
                }
                else {
                    let offsetInMinutes = parseInt(m[5], 10) * 60 + parseInt(m[6] || "0", 10);
                    if (m[4][0] === "-") {
                        offsetInMinutes *= -1;
                    }
                    offset = offsetInMinutes;
                    timeInSeconds -= offsetInMinutes * 60;
                }
            }
            else {
                if (m[1] != null) {
                    baseDate = new Date(m[1]);
                }
                else {
                    const d = new Date();
                    baseDate = new Date(d.getFullYear() + "/" + (d.getMonth() + 1) + "/" + d.getDate());
                }
            }
            date = new Date(baseDate.getTime() + timeInSeconds * 1000);
            // correct for daylight savings time
            date = new Date(date.getTime() + (date.getTimezoneOffset() - baseDate.getTimezoneOffset()) * 60000);
        }
        else {
            fail();
        }
        // Check again the date is valid after transformations, see #2229
        if (isNaN(date.getTime())) {
            fail();
        }
    }
    return [date, offset];
}
export function parse(str, detectUTC = false) {
    const [date, offset] = parseRaw(str);
    // .NET always parses DateTime as Local if there's offset info (even "Z")
    // Newtonsoft.Json uses UTC if the offset is "Z"
    const kind = offset != null
        ? (detectUTC && offset === "Z" ? DateTimeKind.Utc : DateTimeKind.Local)
        : DateTimeKind.Unspecified;
    return DateTime(date.getTime(), kind);
}
export function tryParse(v, defValue) {
    try {
        defValue.contents = parse(v);
        return true;
    }
    catch (_err) {
        return false;
    }
}
export function create(year, month, day, h = 0, m = 0, s = 0, ms = 0, kind) {
    const date = kind === DateTimeKind.Utc
        ? new Date(Date.UTC(year, month - 1, day, h, m, s, ms))
        : new Date(year, month - 1, day, h, m, s, ms);
    if (year <= 99) {
        if (kind === DateTimeKind.Utc) {
            date.setUTCFullYear(year, month - 1, day);
        }
        else {
            date.setFullYear(year, month - 1, day);
        }
    }
    const dateValue = date.getTime();
    if (isNaN(dateValue)) {
        throw new Exception("The parameters describe an unrepresentable Date.");
    }
    return DateTime(dateValue, kind);
}
export function now() {
    return DateTime(Date.now(), DateTimeKind.Local);
}
export function utcNow() {
    return DateTime(Date.now(), DateTimeKind.Utc);
}
export function today() {
    return date(now());
}
export function isLeapYear(year) {
    return year % 4 === 0 && year % 100 !== 0 || year % 400 === 0;
}
export function daysInMonth(year, month) {
    return month === 2
        ? (isLeapYear(year) ? 29 : 28)
        : (month >= 8 ? (month % 2 === 0 ? 31 : 30) : (month % 2 === 0 ? 30 : 31));
}
export function toUniversalTime(date) {
    return date.kind === DateTimeKind.Utc ? date : DateTime(date.getTime(), DateTimeKind.Utc);
}
export function toLocalTime(date) {
    return date.kind === DateTimeKind.Local ? date : DateTime(date.getTime(), DateTimeKind.Local);
}
export function specifyKind(d, kind) {
    return create(year(d), month(d), day(d), hour(d), minute(d), second(d), millisecond(d), kind);
}
export function timeOfDay(d) {
    return hour(d) * 3600000
        + minute(d) * 60000
        + second(d) * 1000
        + millisecond(d);
}
export function date(d) {
    return create(year(d), month(d), day(d), 0, 0, 0, 0, d.kind);
}
export function day(d) {
    return d.kind === DateTimeKind.Utc ? d.getUTCDate() : d.getDate();
}
export function hour(d) {
    return d.kind === DateTimeKind.Utc ? d.getUTCHours() : d.getHours();
}
export function millisecond(d) {
    return d.kind === DateTimeKind.Utc ? d.getUTCMilliseconds() : d.getMilliseconds();
}
export function minute(d) {
    return d.kind === DateTimeKind.Utc ? d.getUTCMinutes() : d.getMinutes();
}
export function month(d) {
    return (d.kind === DateTimeKind.Utc ? d.getUTCMonth() : d.getMonth()) + 1;
}
export function second(d) {
    return d.kind === DateTimeKind.Utc ? d.getUTCSeconds() : d.getSeconds();
}
export function year(d) {
    return d.kind === DateTimeKind.Utc ? d.getUTCFullYear() : d.getFullYear();
}
export function dayOfWeek(d) {
    return d.kind === DateTimeKind.Utc ? d.getUTCDay() : d.getDay();
}
export function dayOfYear(d) {
    const _year = year(d);
    const _month = month(d);
    let _day = day(d);
    for (let i = 1; i < _month; i++) {
        _day += daysInMonth(_year, i);
    }
    return _day;
}
export function add(d, ts) {
    const newDate = DateTime(d.getTime() + ts, d.kind);
    if (d.kind !== DateTimeKind.Utc) {
        const oldTzOffset = d.getTimezoneOffset();
        const newTzOffset = newDate.getTimezoneOffset();
        return oldTzOffset !== newTzOffset
            ? DateTime(newDate.getTime() + (newTzOffset - oldTzOffset) * 60000, d.kind)
            : newDate;
    }
    else {
        return newDate;
    }
}
export function addDays(d, v) {
    return add(d, v * 86400000);
}
export function addHours(d, v) {
    return add(d, v * 3600000);
}
export function addMinutes(d, v) {
    return add(d, v * 60000);
}
export function addSeconds(d, v) {
    return add(d, v * 1000);
}
export function addMilliseconds(d, v) {
    return add(d, v);
}
export function addTicks(d, v) {
    return add(d, toFloat64(v / 10000n));
}
export function addYears(d, v) {
    const newMonth = month(d);
    const newYear = year(d) + v;
    const _daysInMonth = daysInMonth(newYear, newMonth);
    const newDay = Math.min(_daysInMonth, day(d));
    return create(newYear, newMonth, newDay, hour(d), minute(d), second(d), millisecond(d), d.kind);
}
export function addMonths(d, v) {
    const totalMonths = month(d) + v;
    const newMonth = ((totalMonths - 1) % 12 + 12) % 12 + 1;
    const yearOffset = Math.floor((totalMonths - 1) / 12);
    const newYear = year(d) + yearOffset;
    const _daysInMonth = daysInMonth(newYear, newMonth);
    const newDay = Math.min(_daysInMonth, day(d));
    return create(newYear, newMonth, newDay, hour(d), minute(d), second(d), millisecond(d), d.kind);
}
export function subtract(d, that) {
    return typeof that === "number"
        ? add(d, -that)
        : d.getTime() - that.getTime();
}
export function toLongDateString(d) {
    return d.toDateString();
}
export function toShortDateString(d) {
    return d.toLocaleDateString();
}
export function toLongTimeString(d) {
    return d.toLocaleTimeString();
}
export function toShortTimeString(d) {
    return d.toLocaleTimeString().replace(/:\d\d(?!:)/, "");
}
export function equals(d1, d2) {
    return d1.getTime() === d2.getTime();
}
export const compare = compareDates;
export const compareTo = compareDates;
export function op_Addition(x, y) {
    return add(x, y);
}
export function op_Subtraction(x, y) {
    return subtract(x, y);
}
export function isDaylightSavingTime(x) {
    const jan = new Date(x.getFullYear(), 0, 1);
    const jul = new Date(x.getFullYear(), 6, 1);
    return isDST(jan.getTimezoneOffset(), jul.getTimezoneOffset(), x.getTimezoneOffset());
}
function isDST(janOffset, julOffset, tOffset) {
    return Math.min(janOffset, julOffset) === tOffset;
}
export default DateTime;
