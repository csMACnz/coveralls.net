<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:output method="html" doctype-public="-//W3C//DTD HTML 4.01 Transitional//EN"/>
  <xsl:param name="link-suffix" select="'.xml'"/>
  <xsl:template match="/">
    <html lang="en">
      <meta http-equiv="Content-Type" content="text/html; charset=ISO-8859-1"/>
      <style type="text/css">
        table th { text-align: center }
        .hit { background-color: lime; vertical-align: middle }
        .missed { background-color: red; vertical-align: middle }
        .sourceHit { color: green }
        .sourceMissed { color: red }
      </style>
      <title>Coverage Data</title>
      <body>
        <!-- navbar -->
        <div align="center">
        <a href="project{$link-suffix}">Project</a>
        <xsl:call-template name="nbsp">
          <xsl:with-param name="count" select="3"/>
        </xsl:call-template>
        <xsl:choose>
          <xsl:when test="class and class/@namespace != ''">
            <a href="namespace-{class/@namespace}{$link-suffix}">Namespace</a>
          </xsl:when>
          <xsl:otherwise>
            Namespace
          </xsl:otherwise>
        </xsl:choose>
      </div>
        <hr/>
        <!-- caption -->
        <h1 align="center">
          <xsl:if test="project">
            Project
          </xsl:if>
          <xsl:if test="namespace">
            Namespace <xsl:value-of select="namespace/@name"/>
          </xsl:if>
          <xsl:if test="class">
            Class <xsl:value-of select="class/@fullname"/>
          </xsl:if>
        </h1>
        <!-- summary -->
        <table border="2" width="100%">
          <tr>
            <th></th>
            <th>Hit</th>
            <th>Missed</th>
            <th>Coverage</th>
            <th width="50%"></th>
          </tr>
          <!-- root item -->
          <xsl:apply-templates select="./project | ./namespace | ./class"/>
          <!-- child items -->
          <xsl:apply-templates select="/*//project | /*//namespace | /*//class">
            <xsl:sort select="@name"/>
          </xsl:apply-templates>
        </table>
        <!-- source -->
        <xsl:apply-templates select="*/source"/>
      </body>
    </html>
  </xsl:template>
  <xsl:template match="project | namespace | class">
      <tr>
        <td>
          <xsl:call-template name="nbsp">
            <xsl:with-param name="count" select="3"/>
          </xsl:call-template>
          <xsl:variable name="url">
            <xsl:if test="name()='project'">project<xsl:value-of select="$link-suffix"/>
            </xsl:if>
            <xsl:if test="name()='namespace'">namespace-<xsl:value-of select="@name"/><xsl:value-of select="$link-suffix"/>
            </xsl:if>
            <xsl:if test="name()='class'">class-<xsl:value-of select="@fullname"/><xsl:value-of select="$link-suffix"/>
            </xsl:if>
          </xsl:variable>
          <xsl:if test="count(..|/)=1">
            <xsl:value-of select="@name"/>
          </xsl:if>
          <xsl:if test="count(..|/)>1">
            <a href="{$url}">
              <xsl:value-of select="@name"/>
            </a>
          </xsl:if>            
        </td>
        <td align="center"><xsl:value-of select="coverage/@hit"/></td>
        <td align="center"><xsl:value-of select="coverage/@missed"/></td>
        <td align="center"><xsl:value-of select="coverage/@coverage"/>%</td>
        <td>
          <table cellspacing="0" cellpadding="0" width="100%">
            <tr>
              <td>
                <img class="hit" width="{coverage/@coverage}%" src="trans.gif" height="12" alt=""/>
                <img class="missed" width="{100 - coverage/@coverage}%" src="trans.gif" height="12" alt=""/>
              </td>
            </tr>
          </table>
        </td>
      </tr>
  </xsl:template>
  <xsl:template match="source">
    <hr/>
    <pre>
    <code>
      <xsl:apply-templates select="l"/>
    </code>
  </pre>
  </xsl:template>
  <xsl:template match="l">
    <span>
      <xsl:attribute name="class">
        <xsl:if test="@count>0">
          sourceHit
        </xsl:if>
        <xsl:if test="@count=0">
          sourceMissed
        </xsl:if>
      </xsl:attribute>
      <xsl:value-of select="."/>
      <xsl:text disable-output-escaping='yes'>&#xA;</xsl:text>
    </span>
  </xsl:template>
  <xsl:template name="nbsp">
    <xsl:param name="count"/>
    <!-- The Mozilla XSL processor converts &amp;&nbsp' into the string
         '&nbsp;' which is then displayed ??? -->
    <xsl:if test="$count > 0">
      <xsl:choose>
        <xsl:when test="$link-suffix='.html'">
          <xsl:text disable-output-escaping='yes'>&amp;nbsp;</xsl:text>
        </xsl:when>
        <xsl:otherwise>
          <xsl:text>&#160;</xsl:text>
        </xsl:otherwise>
      </xsl:choose>
      <xsl:call-template name="nbsp">
        <xsl:with-param name="count" select="$count - 1"/>
      </xsl:call-template>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>