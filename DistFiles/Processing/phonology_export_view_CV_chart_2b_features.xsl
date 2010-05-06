﻿<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns:xhtml="http://www.w3.org/1999/xhtml"
exclude-result-prefixes="xhtml"
>

  <!-- phonology_export_view_CV_chart_2b_features.xsl 2010-04-20 -->
	<!-- Export to XHTML, Interactive Web page, and at least one feature table. -->
	<!-- Keep the features that distinguish units in the CV chart. -->

	<xsl:output method="xml" version="1.0" encoding="UTF-8" omit-xml-declaration="yes" indent="no" />

	<xsl:variable name="countUnits" select="count(//xhtml:body/xhtml:ul[@class = 'CV chart']/xhtml:li)" />

  <!-- Copy all attributes and nodes, and then define more specific template rules. -->
  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
    </xsl:copy>
  </xsl:template>

	<!-- Keep an articulatory feature only if it distinguishes units. -->
	<!-- That is, at least one unit has it and at least one unit does not. -->
	<xsl:template match="xhtml:table[@class = 'articulatory features']/xhtml:tbody/xhtml:tr">
		<xsl:variable name="feature" select="xhtml:td[@class = 'name']" />
		<xsl:if test="../../../xhtml:table[@class = 'CV chart']//xhtml:ul[@class = 'articulatory features'][xhtml:li[. = $feature]]">
      <xsl:if test="../../../xhtml:table[@class = 'CV chart']//xhtml:ul[@class = 'articulatory features'][not(xhtml:li[. = $feature])]">
				<xsl:copy-of select="." />
			</xsl:if>
    </xsl:if>
  </xsl:template>

	<!-- Keep a univalent binary feature only if it distinguish units. -->
	<!-- That is, at least one unit has it and at least one unit does not. -->
	<xsl:template match="xhtml:table[@class = 'binary features']/xhtml:tbody/xhtml:tr[@class = 'univalent']">
		<xsl:variable name="feature" select="xhtml:td[@class = 'name']" />
		<xsl:if test="../../../xhtml:table[@class = 'CV chart']//xhtml:ul[@class = 'binary features'][xhtml:li[. = $feature]]">
			<xsl:if test="../../../xhtml:table[@class = 'CV chart']//xhtml:ul[@class = 'binary features'][not(xhtml:li[. = $feature])]">
				<xsl:copy-of select="." />
			</xsl:if>
		</xsl:if>
	</xsl:template>

	<!-- Keep a bivalent binary feature only if at least one of its values distinguishes units. -->
	<xsl:template match="xhtml:table[@class = 'binary features']//xhtml:tbody/xhtml:tr[@class = 'bivalent']">
		<xsl:variable name="featureName" select="xhtml:td[@class = 'name']" />
		<xsl:variable name="featurePlus" select="concat('+', $featureName)" />
		<!-- Important: Although cells for minus values might contain en-dash, list items have minus. -->
		<xsl:variable name="featureMinus" select="concat('-', $featureName)" />
		<xsl:variable name="countPlus" select="count(../../../xhtml:table[@class = 'CV chart']//xhtml:ul[@class = 'binary features'][xhtml:li[. = $featurePlus]])" />
		<xsl:variable name="countMinus" select="count(../../../xhtml:table[@class = 'CV chart']//xhtml:ul[@class = 'binary features'][xhtml:li[. = $featureMinus]])" />
		<!--
		<xsl:variable name="countUnspecified" select="count(../../../xhtml:table[@class = 'CV chart']//xhtml:ul[@class = 'binary features'][not(xhtml:li[. = $featurePlus or . = $featureMinus])])" />
		-->
		<xsl:choose>
			<!-- Remove the feature if no unit has either value. -->
			<xsl:when test="$countPlus = 0 and $countMinus = 0" />
			<!-- Remove the feature if all units have the same value. Assume binary values are mutually exclusive. -->
			<xsl:when test="$countPlus = $countUnits or $countMinus = $countUnits" />
			<xsl:otherwise>
				<xsl:copy>
					<xsl:apply-templates select="@*" />
					<xsl:apply-templates select="xhtml:td">
						<xsl:with-param name="countPlus" select="$countPlus" />
						<xsl:with-param name="countMinus" select="$countMinus" />
					</xsl:apply-templates>
				</xsl:copy>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="xhtml:table[@class = 'binary features']//xhtml:td[@class = 'plus']">
		<xsl:param name="countPlus" />
		<xsl:choose>
			<!-- Keep the class attribute and the text if the value distinguishes units. -->
			<xsl:when test="$countPlus != 0">
				<xsl:copy-of select="." />
			</xsl:when>
			<!-- Empty cell. -->
			<xsl:otherwise>
				<xsl:copy />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="xhtml:table[@class = 'binary features']//xhtml:td[@class = 'minus']">
		<xsl:param name="countMinus" />
		<xsl:choose>
			<!-- Keep the class attribute and the text if the value distinguishes units. -->
			<xsl:when test="$countMinus != 0">
				<xsl:copy-of select="." />
			</xsl:when>
			<!-- Empty cell. -->
			<xsl:otherwise>
				<xsl:copy />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<!-- Keep a univalent hierarchical feature if at least one unit has it. -->
	<xsl:template match="xhtml:table[@class = 'hierarchical features']//xhtml:tbody/xhtml:tr[@class = 'univalent']">
		<xsl:variable name="feature" select="xhtml:td[@class = 'name']" />
		<xsl:if test="../../../xhtml:table[@class = 'CV chart']//xhtml:ul[@class = 'hierarchical features'][xhtml:li[. = $feature]]">
			<xsl:copy-of select="." />
		</xsl:if>
	</xsl:template>

	<!-- Keep a bivalent hierarchical feature if at least one unit has a value. -->
	<xsl:template match="xhtml:table[@class = 'hierarchical features']//xhtml:tbody/xhtml:tr[@class = 'bivalent']">
		<xsl:variable name="featureName" select="xhtml:td[@class = 'name']" />
		<xsl:variable name="featurePlus" select="concat('+', $featureName)" />
		<!-- Important: Although cells for minus values might contain en-dash, list items have minus. -->
		<xsl:variable name="featureMinus" select="concat('-', $featureName)" />
		<xsl:variable name="countPlus" select="count(../../../xhtml:table[@class = 'CV chart']//xhtml:ul[@class = 'hierarchical features'][xhtml:li[. = $featurePlus]])" />
		<xsl:variable name="countMinus" select="count(../../../xhtml:table[@class = 'CV chart']//xhtml:ul[@class = 'hierarchical features'][xhtml:li[. = $featureMinus]])" />
		<xsl:if test="$countPlus != 0 or $countMinus != 0">
			<xsl:copy>
				<xsl:apply-templates select="@*" />
				<xsl:apply-templates select="xhtml:td">
					<xsl:with-param name="countPlus" select="$countPlus" />
					<xsl:with-param name="countMinus" select="$countMinus" />
				</xsl:apply-templates>
			</xsl:copy>
		</xsl:if>
	</xsl:template>

	<xsl:template match="xhtml:table[@class = 'hierarchical features']//xhtml:td[@class = 'plus']">
		<xsl:param name="countPlus" />
		<xsl:choose>
			<!-- Keep the class attribute and the text if the value distinguishes units. -->
			<xsl:when test="$countPlus != 0">
				<xsl:copy-of select="." />
			</xsl:when>
			<!-- Empty cell. -->
			<xsl:otherwise>
				<xsl:copy />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="xhtml:table[@class = 'hierarchical features']//xhtml:td[@class = 'minus']">
		<xsl:param name="countMinus" />
		<xsl:choose>
			<!-- Keep the class attribute and the text if the value distinguishes units. -->
			<xsl:when test="$countMinus != 0">
				<xsl:copy-of select="." />
			</xsl:when>
			<!-- Empty cell. -->
			<xsl:otherwise>
				<xsl:copy />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

</xsl:stylesheet>