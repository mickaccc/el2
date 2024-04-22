<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <html>
      <body>
        <h2>My CD Collection</h2>
        <table border="1">
          <tr bgcolor="#9acd32">
            <th style="text-align:left">Title</th>
            <th style="text-align:left">Artist</th>
          </tr>
          <xsl:for-each select="catalog/cd">
            <tr>
              <td>
                <xsl:value-of select="Auftragsnummer"/>
              </td>
              <td>
                <xsl:value-of select="Text"/>
              </td>
            </tr>
          </xsl:for-each>
        </table>

		  <!-- Define a table to display data in. -->
		  <table border="1" cellpadding="3">
			  <tr>
				  <td colspan="5" align="center">
					  <!-- Filter for the project name and display it in a header.  -->
					  <h2>
						  <font face="tahoma" size="5">
							  Status for: <xsl:value-of select="Project/Name" />
						  </font>
					  </h2>
				  </td>
			  </tr>
			  <!-- Define headers for task information. -->
			  <tr>
				  <td colspan="5" align="center">
					  Tasks:
				  </td>
			  </tr>
			  <tr>
				  <th>
					  <font color="black">ID</font>
				  </th>
				  <th>
					  <font color="black">Name</font>
				  </th>
				  <th>
					  <font color="black">Priority</font>
				  </th>
				  <th>
					  <font color="black">Start</font>
				  </th>
				  <th>
					  <font color="black">Finish</font>
				  </th>
			  </tr>
			  <!-- Filter for tasks -->
			  <xsl:for-each select="Project/Tasks/Task">
				  <!-- Exclude summary tasks -->
				  <xsl:if test="Summary[.=0]">
					  <xsl:choose>
						  <!-- Display information for critical tasks with a colored background. -->
						  <xsl:when test="Critical[.=1]">
							  <tr>
								  <td>
									  <xsl:value-of select="ID"/>
								  </td>
								  <td>
									  <b>
										  <xsl:value-of select="Name"/>
									  </b>
								  </td>
								  <td>
									  <b>
										  <xsl:value-of select="Priority"/>
									  </b>
								  </td>
								  <td>
									  <b>
										  <xsl:value-of select="Start"/>
									  </b>
								  </td>
								  <td>
									  <b>
										  <xsl:value-of select="Finish"/>
									  </b>
								  </td>
							  </tr>
						  </xsl:when>
						  <!-- Display information for noncritical tasks with a white background. -->
						  <xsl:otherwise>
							  <tr>
								  <td>
									  <xsl:value-of select="ID"/>
								  </td>
								  <td>
									  <xsl:value-of select="Name"/>
								  </td>
								  <td>
									  <xsl:value-of select="Priority"/>
								  </td>
								  <td>
									  <xsl:value-of select="Start"/>
								  </td>
								  <td>
									  <xsl:value-of select="Finish"/>
								  </td>
							  </tr>
						  </xsl:otherwise>
					  </xsl:choose>
				  </xsl:if>
			  </xsl:for-each>
			  <!-- Define headers for overallocated resource information. -->
			  <tr>
				  <td colspan="5" align="center">
					  Overallocated Resources:
				  </td>
			  </tr>
			  <tr>
				  <th>
					  <font color="black">ID</font>
				  </th>
				  <th colspan="2">
					  <font color="black">Name</font>
				  </th>
				  <th colspan="2">
					  <font color="black">Overtime Rate</font>
				  </th>
			  </tr>
			  <!-- Filter for resources -->
			  <xsl:for-each select="Project/Resources/Resource">
				  <!-- Sort resources alphabetically by name -->
				  <xsl:sort select="Name" />
				  <!-- Display information for only resources that are overallocated. -->
				  <xsl:if test="OverAllocated[.=1]">
					  <tr>
						  <td>
							  <xsl:value-of select="ID"/>
						  </td>
						  <td  colspan="2">
							  <xsl:value-of select="Name"/>
						  </td>
						  <td  colspan="2" align="center">
							  $<xsl:value-of select="OvertimeRate"/>.00
						  </td>
					  </tr>
				  </xsl:if>
			  </xsl:for-each>
		  </table>
		  
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
