import { spawn as spawnSync } from 'node:child_process';

export const spawn = (cmd, options) => {
    const segments = cmd.split(" ");
    const command = segments.shift();

    const child = spawnSync(command, segments, options);

    const promise = new Promise((resolve, reject) => {
        child.on('error', reject)

        child.on('exit', code => {
            if (code === 0) {
                resolve()
            } else {
                const err = new Error(`child exited with code ${code}`)
                err.code = code
                reject(err)
            }
        })
    })

    promise.child = child

    return promise
}

export const simpleSpawn = (cmd, cwd) => {
    return spawn(
        cmd,
        {
            stdio: "inherit",
            shell: true,
            cwd: cwd
        }
    )
}
