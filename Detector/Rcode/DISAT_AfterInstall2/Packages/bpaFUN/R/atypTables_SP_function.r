##' This function creates the atypical tables for each month
##' @export
##' @param sigPath
##'
##' @return atypicality tables
##'
##' @author Brett Amidan
##'
##' @examples
##' atypTables()

atypTables_SP <- function(month="2013-01",gasMatrix,sigPath,outputPath,
  analysisTypes,atypCutoff,clusterOutput,top=10) {

  require(hwriter)

  ### create the output folder
  newOutput <- paste(outputPath,"/",month,sep="")
  dir.create(newOutput,showWarnings=FALSE,recursive=TRUE)

  ## trim gasMatrix to month of interest
  indy <- is.element(substring(rownames(gasMatrix),1,7),month)
  gasMatrix <- gasMatrix[indy,]

  ## list of different analysis groups
  groups <- colnames(gasMatrix)
  ###### Plot gas scores
  for (i in 1:length(groups)) {
    jpeg(file=paste(newOutput,"/",groups[i],".jpg",sep=""),width=7,height=4,
      units="in",res=150)
    plot(as.POSIXct(rownames(gasMatrix)),gasMatrix[,i],xlab=month,
      ylab="Atypicality Score",main=analysisTypes[[i]]$title,type="l",
      ylim=c(0,30))
    abline(h=atypCutoff[i],col="red",lty=2)
    dev.off()
  } # ends i

  ###### TRIM gasMatrix to remove consecutive atypicals
  ind1 <- gasMatrix>matrix(atypCutoff,nrow=nrow(gasMatrix),ncol=ncol(gasMatrix),byrow=TRUE) &
    !is.na(gasMatrix)
  indy <- rowSums(ind1)>0
  atypTimes <- rownames(gasMatrix)[indy]
  atypTimes <- sort(unique(atypTimes))
  ## only keep first time (not consecutive ones)
  atypTimes2 <- keepFirst(timeVec=atypTimes)
  ind <- !is.element(atypTimes,atypTimes2)
  rmTimes <- atypTimes[ind]
  ## remove times
  ind2 <- is.element(rownames(gasMatrix),rmTimes)
  gasMatrix <- gasMatrix[!ind2,]
  gasMatrix <- round(gasMatrix,2)

  ### Loop thru each group
  for (i in 1:length(groups)) {
    atypTab <- cbind(gasMatrix[,groups[i]],rep("",nrow(gasMatrix)))
    colnames(atypTab) <- c("AtypicalityScore","Rationale")
    rownames(atypTab) <- rownames(gasMatrix)
    ## remove NAs
    ind.na <- is.na(atypTab[,"AtypicalityScore"])
    atypTab <- atypTab[!ind.na,]
    ## only continue if gas scores exist
    if (nrow(atypTab)>0) {
      ## add cluster column
      Cluster <- clusterOutput$membership[rownames(atypTab)]
      ## add link to Cluster
      Cluster <- paste('<a href=\"file:///',outputPath,'/Clusters/',
        Cluster,'.html\">',Cluster,'</a>',sep="")
      ## finish table
      atypTab <- cbind(rownames(atypTab),Cluster,atypTab)
      colnames(atypTab) <- c("Date","Cluster","AtypicalityScore","Rationale")
      rownames(atypTab) <- NULL

      ## sort
      ind.sort <- sort(as.numeric(atypTab[,"AtypicalityScore"]),decreasing=TRUE,
        index.return=TRUE)$ix
      atypTab <- atypTab[ind.sort,]
      rownames(atypTab) <- NULL
      ## loop thru each row to add links and rationale
      if (min(200,sum(as.numeric(atypTab[,"AtypicalityScore"])>atypCutoff[i]))>0) {
      for (j in 1:min(200,sum(as.numeric(atypTab[,"AtypicalityScore"])>atypCutoff[i]))) {
        ## make sure folder exists
        check1 <- is.element(substring(atypTab[j,"Date"],1,10),dir(outputPath))
        if (check1) {
          check2 <- is.element(gsub(":","",substring(atypTab[j,"Date"],12)),
            dir(paste(outputPath,"/",substring(atypTab[j,"Date"],1,10),sep="")))
          if (check2) {
            ## results exist, so add links
            ## atypicality plot
            atypTab[j,"AtypicalityScore"] <- paste('<a href="file:///',
              outputPath,'/',substring(atypTab[j,"Date"],1,10),
              '/',gsub(":","",substring(atypTab[j,"Date"],12)),"/",groups[i],
              '.jpg">',atypTab[j,"AtypicalityScore"],'</a>',sep="")
          }
        }
        ## add rationale
        folder <- paste(outputPath,"/",substring(atypTab[j,"Date"],1,10),"/",
          substring(atypTab[j,"Date"],12,13),substring(atypTab[j,"Date"],15,16),
          sep="")
        load(paste(folder,"/Rationale.Rdata",sep=""))
        paragraph <- Rationale[[groups[i]]]$paragraph
        variables <- Rationale[[groups[i]]]$variables
        variables <- variables[1:min(length(variables),top)]
        if (length(variables)>0) {
        for (k in 1:length(variables)) {
          paragraph <- gsub(variables[k],paste('<a href=\"file:///',folder,'/',
            variables[k],'.jpg\">',variables[k],'</a>',sep=""),paragraph,fixed=TRUE)
        }
        } else {
          paragraph <- ""
        }
        atypTab[j,"Rationale"] <- paragraph
      } # ends j
      ### plot link
      plotLink <- paste("<img src='file:///",newOutput,"/",groups[i],".jpg' />",
        sep="")

      ###### Make the html table  #################
      p <- openPage(paste(groups[i],".html",sep=""),
        dirname=newOutput,link.css='http://www.ebi.ac.uk/~gpau/hwriter/hwriter.css')
  #    hwrite(analysisTypes[[i]]$title, p, center=TRUE, heading=1)
      hwrite(plotLink,p)
      hwrite(atypTab[1:100,], p, row.bgcolor='#ffffaa')
      closePage(p)
    } else {
      p <- openPage(paste(groups[i],".html",sep=""),
        dirname=newOutput,link.css='http://www.ebi.ac.uk/~gpau/hwriter/hwriter.css')
      hwrite(analysisTypes[[i]]$title, p, center=TRUE, heading=1)
      hwrite("No Data",p)
      closePage(p)
    }
    } else { 
      p <- openPage(paste(groups[i],".html",sep=""),
        dirname=newOutput,link.css='http://www.ebi.ac.uk/~gpau/hwriter/hwriter.css')
      hwrite(analysisTypes[[i]]$title, p, center=TRUE, heading=1)
      hwrite("No Data",p)
      closePage(p)
    } # ends else/if
  } # ends i
  
  invisible()

} # ends function
