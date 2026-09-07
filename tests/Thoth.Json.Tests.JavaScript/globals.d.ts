// Fable's TypeScript output and Scriptorium read Node globals. Declared here
// rather than through @types/node so the repository stays free of a package.json.
declare const NO_COLOR: string | undefined

declare const process: {
    argv: string[]
    env: Record<string, string | undefined>
    cwd(): string
    exit(code?: number): never
    stdout: { write(text: string): void }
    stderr: { write(text: string): void }
}
