##' This function translates the colnames
##' @export
##' @param nameVec is the colnames for the data
##'
##' @return matrix with the parts of the column names
##'
##' @author Brett Amidan
##'
##' @examples
##' colnameTranslator(colnames(data))

colnameTranslator <- function(nameVec) {

  ## Make the output
  cnames <- c("PMU","Bus","Variable","VarType")
  out <- matrix("",nrow=length(cnames),ncol=length(nameVec))
  colnames(out) <- nameVec
  rownames(out) <- cnames
  
  ## loop thru each name and get the parts
  for (i in 1:length(nameVec)) {
    temp <- unlist(strsplit(nameVec[i],".",fixed=TRUE))
    if (length(temp)>2) {
      temp <- c(temp[1],paste(temp[2:length(temp)],collapse="."))
    }
    ## get PMU
    out["PMU",i] <- temp[1]
    ## get Bus if exists
    if (grepl("_",temp[2],fixed=TRUE)) {
      t2 <- unlist(strsplit(temp[2],"_",fixed=TRUE))
      out["Bus",i] <- t2[1]
      temp[2] <- t2[length(t2)]
    }
    ## get variable
    out["Variable",i] <- temp[2]
    
    ## get variable type
    if (temp[2]=="stat") out["VarType",i] <- "stat"
    if (grepl("MAG",temp[2],fixed=TRUE) & grepl("VP",temp[2],fixed=TRUE))
      out["VarType",i] <- "V"
    if (grepl("ANG",temp[2],fixed=TRUE) & grepl("VP",temp[2],fixed=TRUE))
      out["VarType",i] <- "VA"
    if (grepl("MAG",temp[2],fixed=TRUE) & grepl("VA",temp[2],fixed=TRUE))
      out["VarType",i] <- "V_PhaseA"
    if (grepl("ANG",temp[2],fixed=TRUE) & grepl("VA",temp[2],fixed=TRUE))
      out["VarType",i] <- "VA_PhaseA"
    if (grepl("MAG",temp[2],fixed=TRUE) & grepl("VB",temp[2],fixed=TRUE))
      out["VarType",i] <- "V_PhaseB"
    if (grepl("ANG",temp[2],fixed=TRUE) & grepl("VB",temp[2],fixed=TRUE))
      out["VarType",i] <- "VA_PhaseB"
    if (grepl("MAG",temp[2],fixed=TRUE) & grepl("VC",temp[2],fixed=TRUE))
      out["VarType",i] <- "V_PhaseC"
    if (grepl("ANG",temp[2],fixed=TRUE) & grepl("VC",temp[2],fixed=TRUE))
      out["VarType",i] <- "VA_PhaseC"
    if (grepl("MAG",temp[2],fixed=TRUE) & grepl("I",temp[2],fixed=TRUE))
      out["VarType",i] <- "I"
    if (grepl("ANG",temp[2],fixed=TRUE) & grepl("I",temp[2],fixed=TRUE))
      out["VarType",i] <- "IA"
    if (grepl("MW",temp[2],fixed=TRUE)) out["VarType",i] <- "MW"
    if (grepl("MV",temp[2],fixed=TRUE)) out["VarType",i] <- "MV"
    if (temp[2]=="freq") out["VarType",i] <- "freq"
    if (temp[2]=="rocof") out["VarType",i] <- "rocof"
    if (grepl("ANG.DA",temp[2],fixed=TRUE)) out["VarType",i] <- "DA"
    if (grepl("ANG.NA",temp[2],fixed=TRUE)) out["VarType",i] <- "NA"
    if (grepl("ANG.MA",temp[2],fixed=TRUE)) out["VarType",i] <- "MA"
    if (grepl("ANG.SA",temp[2],fixed=TRUE)) out["VarType",i] <- "SA"

    
  } # ends i
  out
} # ends function
