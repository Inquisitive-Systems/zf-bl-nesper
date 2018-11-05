# ZoneFox BL NEsper repo 

Consumes ZoneFox events from MSMQ queue and runs events against rules in the NEsper engine.

# Updating this repo

## One-time setup
Add a remote repo
```
cd c:\git\zf
git remote add zf-bl-nesper https://github.com/Inquisitive-Systems/zf-bl-nesper.git
git remote -v 
```

Expected output
```
origin  https://github.com/Inquisitive-Systems/zf.git (fetch)
origin  https://github.com/Inquisitive-Systems/zf.git (push)
zf-bl-nesper    https://github.com/Inquisitive-Systems/zf-bl-nesper.git (fetch)
zf-bl-nesper    https://github.com/Inquisitive-Systems/zf-bl-nesper.git (push)
```

## Making changes

- Checkout `ZF` repo
- Make a change in `ZF` repo
- Push your changes to `zf-bl-nesper` repo using
    ```
    git subtree push --prefix=src/apps/ZF.BL.Nesper zf-bl-nesper master
    ```

# License
GNU GPL v2