module Utils

    let removeLeadingTrailing startChar endChar (input:string) =
        let temp = input.Trim()

        if temp.Length < 2 then
            temp
        elif temp.[0] = startChar && temp.[temp.Length - 1] = endChar then
            temp.[1..temp.Length-2]
        else
            temp
        

                
    let removeLeadingTrailingDoubleQuote (input:string) =
        removeLeadingTrailing '"' '"' input
