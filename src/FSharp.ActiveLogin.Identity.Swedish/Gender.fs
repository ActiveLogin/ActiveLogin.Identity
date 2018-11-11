namespace ActiveLogin.Identity.Swedish

/// <summary>
/// Specifies possible genders (juridiskt kön) in sweden.
/// </summary>
type Gender = 
    /// <summary>
    /// Other gender
    /// </summary>
    | NonBinary = 0

    /// <summary>
    /// Gender representing a female
    /// </summary>
    | Female = 1

    /// <summary>
    /// Gender representing a male
    /// </summary>
    | Male = 2
